using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Identity.API.Infrastructure.Behaviors
{
    /// <summary>
    /// 统一的ValidationBehavior，支持处理所有类型的请求
    /// 这个实现可以处理：
    /// - IRequest&lt;TResponse&gt; 类型的请求（有返回值）
    /// - IRequest 类型的请求（无返回值，返回Unit）
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> logger;
        
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger) 
        {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await ValidateRequest(request, cancellationToken);
            return await next();
        }

        private async Task ValidateRequest(TRequest request, CancellationToken cancellationToken)
        {
            var requestType = typeof(TRequest).Name;
            logger.LogInformation($"开始验证请求: {requestType}");

            if (validators != null && validators.Any())
            {
                logger.LogInformation($"找到 {validators.Count()} 个验证器用于 {requestType}");
                
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
                    logger.LogWarning($"验证失败: {requestType} - {string.Join(", ", errorMessages)}");
                    throw new ValidationException(failures);
                }
                
                logger.LogInformation($"验证通过: {requestType}");
            }
            else
            {
                logger.LogWarning($"未找到 {requestType} 的验证器");
            }
        }
    }
}
