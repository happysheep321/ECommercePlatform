using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.Aggregates.UserAggregate
{
    public class UserLoginLog : Entity<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// 用户实体
        /// </summary>
        public User User { get; private set; } = default!;

        /// <summary>
        /// 登录时间（UTC）
        /// </summary>
        public DateTime LoginTime { get; private set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string? IP { get; private set; }

        /// <summary>
        /// 登录设备信息
        /// </summary>
        public string? Device { get; private set; }

        /// <summary>
        /// 登录地理位置
        /// </summary>
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
