using FluentValidation;
using MediatR;

namespace ECommerce.Identity.API.Infrastructure.Behaviors
{
    public class ValidationBehavior<TRequest,TResponse>:IPipelineBehavior<TRequest, TResponse> where TRequest:IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators != null)
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
                    throw new ValidationException($"请求验证失败: {string.Join(", ", errorMessages)}");
                }
            }

            return await next();
        }
    }
}
