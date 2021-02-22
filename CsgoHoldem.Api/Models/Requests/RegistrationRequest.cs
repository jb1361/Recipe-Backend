using System.ComponentModel.DataAnnotations;
using System.Linq;
using CsgoHoldem.Api.Models.Context;
using CsgoHoldem.Api.Util;

namespace CsgoHoldem.Api.Models.Requests
{
    public class RegistrationRequest
    {
        [RegularExpression(@"^.+@.+\..+$", ErrorMessage = "Must be a valid email address")]
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public int RoleId { get; set; }
        public string ConfirmPassword { get; set; }
        
        public GenericErrorResponse Validate(DefaultContext dbContext)
        {
            if (!string.IsNullOrEmpty(Email) && dbContext.Users.Count(u => u.Email == Email) != 0)
            {
                return new GenericErrorResponse { Message = "The specified personal email is already in use"};
            }
        
            if (Password != ConfirmPassword)
            {
                return new GenericErrorResponse { Message = "Passwords do not match"};
            }
        
            return null;
        }
        public bool IsValid(DefaultContext dbContext)
        {
            return Validate(dbContext) == null;
        }

        public string GetEmail()
        {
            return this.Email;
        }
    }
}