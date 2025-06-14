using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerce.SharedKernel.Enums;

namespace ECommerce.BuildingBolcks.Authentication
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings settings;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            settings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                         ?? throw new ArgumentNullException("未找到 JwtSettings 配置节或配置无效");
        }

        /// <summary>
        /// 创建 JWT 令牌
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="userName">用户名</param>
        /// <param name="userType">用户类型</param>
        /// <param name="email">电子邮箱</param>
        /// <param name="phoneNumber">移动电话</param>
        /// <returns></returns>
        public (string Token, DateTime Expiration) GenerateToken(Guid userId, string userName, UserType userType, string email, string phoneNumber)
        {
            var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(settings.ExpiresInHours));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, userType.ToString()), // 默认角色为 User
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, phoneNumber ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expires);
        }

        public (string RefreshToken, DateTime Expiration) GenerateRefreshToken()
        {
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(settings.RefreshTokenExpiresInDays));
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            return (refreshToken, expires);
        }
    }
}
