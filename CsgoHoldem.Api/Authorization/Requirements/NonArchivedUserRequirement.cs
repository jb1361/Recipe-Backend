using System.Threading.Tasks;
using CsgoHoldem.Api.Models.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CsgoHoldem.Api.Authorization.Requirements
{
    public class NonArchivedUserRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsUserArchived(AuthorizationHandlerContext context, DefaultContext databaseContext)
        {
            var user = await databaseContext.Users.SingleOrDefaultAsync(u => u.Id == int.Parse(context.User.Identity.Name));
            return user.IsArchived();
        }
    }
}