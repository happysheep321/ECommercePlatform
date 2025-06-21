namespace ECommerce.BuildingBolcks.Authentication
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double ExpiresInHours { get; set; } = 2;
        public double RefreshTokenExpiresInDays { get; set; } = 7;
    }
}
