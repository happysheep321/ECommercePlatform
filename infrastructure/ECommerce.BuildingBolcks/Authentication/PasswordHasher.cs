using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ECommerce.BuildingBlocks.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        // 将明文密码进行哈希，返回格式为 "盐.哈希"
        public string HashPassword(string password)
        {
            // 生成一个128位（16字节）的随机盐，用于防止彩虹表攻击
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            // 使用PBKDF2算法对密码进行哈希，带上盐，进行10000次迭代
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,                  // 明文密码
                salt: salt,                         // 随机盐
                prf: KeyDerivationPrf.HMACSHA256,  // 伪随机函数，使用HMACSHA256
                iterationCount: 10000,              // 迭代次数，越大越安全但越慢
                numBytesRequested: 256 / 8));       // 输出哈希的字节数，这里是256位（32字节）

            // 把盐和哈希转换成Base64字符串，并用"."拼接，方便存储和后续验证
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        // 验证明文密码是否与已存的哈希密码匹配
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // 把存储的字符串按"."拆分成盐和哈希两部分
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
                return false; // 格式不对直接返回验证失败

            // 从Base64解码出盐的字节数组
            var salt = Convert.FromBase64String(parts[0]);

            // 已存的哈希部分（Base64字符串）
            var expectedHash = parts[1];

            // 用同样的算法，用输入的密码和同样的盐生成新的哈希
            string actualHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: providedPassword,          // 用户输入的密码
                salt: salt,                         // 存储的盐
                prf: KeyDerivationPrf.HMACSHA256,  // 算法
                iterationCount: 10000,              // 迭代次数
                numBytesRequested: 256 / 8));       // 输出哈希字节数

            // 比较新算的哈希和存储哈希是否相等
            return actualHash == expectedHash;
        }
    }
}
