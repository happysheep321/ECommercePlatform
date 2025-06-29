using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserLoginLog : Entity<Guid>
    {
        public Guid UserId { get; private set; }

        public User User { get; private set; } = default!;

        public DateTime LoginTime { get; private set; }

        public string? IP { get; private set; }

        public string? Device { get; private set; }

        public string? Location { get; private set; }

        protected UserLoginLog() { }

        public UserLoginLog(Guid userId, string ip, string? device = null, string? location = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            LoginTime = DateTime.UtcNow;
            IP = ip;
            Device = device;
            Location = location;
        }
    }
}
