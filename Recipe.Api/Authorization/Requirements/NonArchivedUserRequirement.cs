using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Models.Context;

namespace Recipe.Api.Authorization.Requirements
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