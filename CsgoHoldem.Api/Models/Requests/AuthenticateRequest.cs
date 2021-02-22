namespace CsgoHoldem.Api.Models.Requests
{
    public class AuthenticateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}