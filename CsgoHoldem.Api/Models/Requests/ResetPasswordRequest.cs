namespace CsgoHoldem.Api.Models.Requests
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string PasswordResetToken { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}