namespace BookNest.Services
{
    public class GenerateOtpService
    {
        public static string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10).ToString();
            }

            return otp;
        }
    }
}
