namespace BookNest.Models
{
    public class JwtTokenInfo
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required double ExpiryInMinute { get; set; }
        public required string SecretKey { get; set; }
    }
}
