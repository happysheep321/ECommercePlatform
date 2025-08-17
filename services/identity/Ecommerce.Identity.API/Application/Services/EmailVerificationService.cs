using ECommerce.Identity.API.Application.Interfaces;
using ECommerce.BuildingBlocks.Redis;
using System.CodeDom.Compiler;
using System.Text;

namespace ECommerce.Identity.API.Application.Services
{
    public class EmailVerificationService:IVerificationCodeService
    {
        private readonly IRedisHelper redisHelper;
        private readonly IEmailSender emailSender;
        private readonly ILogger<EmailVerificationService> logger;

        private const int CodeLength = 6;
        private const int ExpiryMinutes = 5;
        private readonly TimeSpan operationTimeout = TimeSpan.FromSeconds(10);

        public EmailVerificationService(IRedisHelper redisHelper, IEmailSender emailSender, ILogger<EmailVerificationService> logger)
        {
            this.redisHelper = redisHelper;
            this.emailSender = emailSender;
            this.logger = logger;
        }

        public async Task<bool> SendCodeAsync(string email)
        {
            try
            {
                using var cts = new CancellationTokenSource(operationTimeout);
                var code = GenerateCode(CodeLength);
                var key=GetRedisKey(email);

                await redisHelper.SetAsync(key, code, TimeSpan.FromMinutes(ExpiryMinutes));
                logger.LogInformation($"验证码 {code} 已发送至 {email}，有效期 {ExpiryMinutes} 分钟。");
                return await emailSender.SendEmailAsync(email, "您的验证码", $"您的验证码是：{code}，请在{ExpiryMinutes}分钟内使用。");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "发送验证码失败: {Email}", email);
                throw new InvalidOperationException("验证码发送失败，请稍后重试");
            }
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            try
            {
                using var cts = new CancellationTokenSource(operationTimeout);
                var key = GetRedisKey(email);
                var cachedCode = await redisHelper.GetAsync<string>(key);

                if (cachedCode == null || cachedCode != code)
                {
                    logger.LogWarning("验证码验证失败: {Email}, 输入码: {InputCode}, 缓存码: {CachedCode}", 
                        email, code, cachedCode ?? "null");
                    return false;
                }

                await redisHelper.DeleteAsync(key);
                logger.LogInformation("验证码验证成功: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "验证码验证过程中发生错误: {Email}", email);
                throw new InvalidOperationException("验证码验证失败，请稍后重试");
            }
        }

        private static string GenerateCode(int length)
        {
            var random = new Random();
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(random.Next(10)); // 0-9 的随机数字
            }
            return sb.ToString();
        }

        private static string GetRedisKey(string email)
        {
            return $"email:code:{email}";
        }
    }
}
