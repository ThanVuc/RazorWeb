using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RazorWeb.Model;
namespace App.Admin.Role
{
    [Authorize(Policy = "AdministratorPermission")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(webBlogContext, logger, roleManager, userManager)
        {
        }

        public class RoleModel : IdentityRole
        {
            public string[]? Claims { get; set; }
        }

        public List<RoleModel> Roles { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            Roles = new List<RoleModel>();

            foreach (var r in roles)
            {
                var claims = (await _roleManager.GetClaimsAsync(r));
                var listClaims = claims.Select(c => c.Type + " = " + c.Value);

                var RoleTemp = new RoleModel()
                {
                    Name = r.Name,
                    Id = r.Id,
                    Claims = listClaims.ToArray()
                };
                Roles.Add(RoleTemp);
            }

            return Page();
        }
    }
}
