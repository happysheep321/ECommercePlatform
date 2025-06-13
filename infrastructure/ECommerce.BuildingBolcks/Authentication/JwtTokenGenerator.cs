using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BuildingBolcks.Authentication
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public (string Token,DateTime Expiration) GenerateToken(Guid userId, string userName, string email, string phoneNumber)
        {
            // 这里可以使用 JWT 库生成令牌
            // 例如使用 System.IdentityModel.Tokens.Jwt 或其他库
            // 这里只是一个示例，实际实现需要根据你的需求来
            var token = $"{userName}.{userId}.{email}.{phoneNumber}"; // 简单的示例，实际应使用加密和签名
            var expiration = DateTime.UtcNow.AddHours(1); // 设置过期时间为1小时
            return (token, expiration);
        }
    }
}
