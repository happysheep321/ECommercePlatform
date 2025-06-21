namespace Ecommerce.Identity.API.Application.Interfaces
{
    public interface ISmsCodeService
    {
        Task SendRegisterCodeAsync(string phone);

        Task<bool> VerifyCodeAsync(string phone, string code);
    }
}
