namespace Ecommerce.Identity.API.Application.Interfaces
{
    public interface IVerificationCodeService
    {
        Task<bool> SendCodeAsync(string email);
        Task<bool> VerifyCodeAsync(string email, string code);
    }
}
