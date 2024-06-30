using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RazorWeb.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace App.Admin.Role
{
    [Authorize(Policy = "FullControlPermisstion")]
    public class AddClaims : RolePageModel
    {
        public AddClaims(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(webBlogContext, logger, roleManager, userManager)
        {
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Require Fill Claim Type")]
            [Display(Name = "Claim Type")]
            [StringLength(100, ErrorMessage = "{0} From {2} To {1} Char.")]
            public string? ClaimType { set; get; }

            [Required(ErrorMessage = "Require Fill Claim Value")]
            [Display(Name = "Claim Value")]
            [StringLength(100, ErrorMessage = "{0} From {2} To {1} Char.")]
            public string? ClaimValue { set; get; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        [FromRoute]
        public string roleID { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var role = await _roleManager.FindByIdAsync(roleID);
            if (role == null)
            {
                return Content("Not Found Role ID");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = await _roleManager.FindByIdAsync(roleID);
            if (role == null)
            {
                return Content("Not Found Role ID");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if ((await _roleManager.GetClaimsAsync(role)).Any(rc => rc.Type == Input.ClaimType && rc.Value == Input.ClaimValue))
            {
                ModelState.TryAddModelError(string.Empty, "Claim has been existed");
                return Page();
            }

            Claim newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);

            if (!result.Succeeded)
            {
                StatusMessage = "Error: Add Claim Fail!";

                foreach (var err in result.Errors)
                {
                    ModelState.TryAddModelError(string.Empty,err.Description);
                }
            }

            return RedirectToPage("./CreateOrUpdate","StartUpdateRole", new {roleID = role.Id});
        }
    }
}
