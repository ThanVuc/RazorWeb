using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RazorWeb.Model;
using System.Security.Claims;

namespace RazorWeb.Sercurity.Requirement
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;
        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, 
            UserManager<AppUser> userManager )
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirement = context.PendingRequirements.ToList();
            var rs = context.Resource?.GetType().Name;
            _logger.LogInformation(rs);
            foreach (var requirement in pendingRequirement)
            {
                if (requirement is IsGenZRequirement)
                {
                    if (await IsGenZ(context.User, requirement))
                    {
                        _logger.LogInformation("IsGenZ Success");
                        context.Succeed(requirement);
                    }
                    else
                    {
                        _logger.LogInformation("IsGenZ Fail");
                    }
                } else
                if (requirement is ArticleUpdateRequirement)
                {
                    if (await CheckExpireArticleUpdate(context.User,context.Resource,requirement))
                    {
                        context.Succeed(requirement);
                    } else
                    {
                        _logger.LogInformation("Article is Expired");
                    }
                }
            }
        }

        private async Task<bool> CheckExpireArticleUpdate(ClaimsPrincipal user, object? resource, IAuthorizationRequirement requirement)
        {
            var appUser = await _userManager.GetUserAsync(user);

            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin Update");
                return true;
            }

            var article = (Article)resource;

            var articleUpdateRequirement = (ArticleUpdateRequirement)requirement;

            DateTime date = new DateTime(articleUpdateRequirement.Year,
                articleUpdateRequirement.Month,articleUpdateRequirement.Date);

            return article.Created >= date;
        }

        private async Task<bool> IsGenZ(ClaimsPrincipal user, IAuthorizationRequirement requirement)
        {
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser.Birth == null)
            {
                return false;
            }
            var birthYear = appUser.Birth.Value.Year;
            var genZ = (IsGenZRequirement)requirement; 

            return genZ.FromYear <= birthYear && birthYear <= genZ.ToYear;
        }
    }
}
