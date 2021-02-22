using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Recipe.Api.Authorization.Requirements;
using Recipe.Api.Models.Context;

namespace Recipe.Api.Authorization.Handlers
{
    public class NonArchivedUserHandler : AuthorizationHandler<NonArchivedUserRequirement>
    {
        private readonly DefaultContext _databaseContext;
        
        public NonArchivedUserHandler(DefaultContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            NonArchivedUserRequirement requirement)
        {
            if (await requirement.IsUserArchived(context, _databaseContext))
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }
        }
    }
}