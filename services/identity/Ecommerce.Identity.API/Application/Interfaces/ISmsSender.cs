namespace Ecommerce.Identity.API.Application.Interfaces
{
    public interface ISmsSender
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
