using Ecommerce.Identity.API.Application.Interfaces;
using ECommerce.BuildingBolcks.Redis;
using StackExchange.Redis;

namespace Ecommerce.Identity.API.Application.Services
{
    public class SmsCodeService : ISmsCodeService
    {
        private readonly IRedisHelper redisHelper;
        private readonly ISmsSender smsSender;

        public SmsCodeService(IRedisHelper redisHelper,ISmsSender smsSender)
        {
            this.redisHelper = redisHelper;
            this.smsSender = smsSender;
        }

        public  async Task SendRegisterCodeAsync(string phone)
        {
            var code = new Random().Next(100000,999999).ToString();
            var cacheKey = $"sms:register:{phone}";
            await redisHelper.SetAsync(cacheKey, code, TimeSpan.FromMinutes(5));
            await smsSender.SendAsync(phone, $"您的注册验证码是：{code}，请在5分钟内使用");
        }

        public async Task<bool> VerifyCodeAsync(string phone, string code)
        {
            var cacheKey = $"sms:register:{phone}";
            var storedCode = await redisHelper.GetAsync<string>(cacheKey);
            return storedCode != null && storedCode == code;
        }
    }
}
