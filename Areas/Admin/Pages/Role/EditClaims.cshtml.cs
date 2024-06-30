using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RazorWeb.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace App.Admin.Role
{
    [Authorize(Policy = "AdministratorPermission")]
    public class EditClaims : RolePageModel
    {
        public EditClaims(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(webBlogContext, logger, roleManager, userManager)
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
        public int ClaimID { get; set; }

        public IdentityRole Role { get; set; }

        public IdentityRoleClaim<string> Claim { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            Claim = (_context.RoleClaims.Where(c => c.Id == ClaimID)).FirstOrDefault();
            if (Claim == null)
            {
                return Content("Not Found Claim");
            }

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);

            if (Role == null)
            {
                return Content("Not Found Role");
            }

            Input = new InputModel()
            {
                ClaimType = Claim.ClaimType,
                ClaimValue = Claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Claim = (_context.RoleClaims.Where(c => c.Id == ClaimID)).FirstOrDefault();
            if (Input == null)
            {
                return Content("Not Found Input");
            }

            if (Claim == null)
            {
                return Content("Not Found Claim");
            }

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);

            if (Role == null)
            {
                return Content("Not Found Claim ID");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.RoleClaims.Any(rc => rc.RoleId == Role.Id && rc.ClaimType == Input.ClaimType && rc.ClaimValue == Input.ClaimValue))
            {
                ModelState.TryAddModelError(string.Empty, "Claim has been existed");
                return Page();
            }

            //input 2
            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Has Just Updated Claim!";

            return RedirectToPage("./CreateOrUpdate","StartUpdateRole", new {roleID = Role.Id});


        }

        public async Task<IActionResult> OnPostDeleteClaimsAsync()
        {
            Claim = (_context.RoleClaims.Where(c => c.Id == ClaimID)).FirstOrDefault();
            if (Claim == null)
            {
                return Content("Claim is Not Found");
            }

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null)
            {
                return Content("Role is Not Found");
            }
            Claim deleteClaim = new Claim(Claim.ClaimType, Claim.ClaimValue);
            var result = await _roleManager.RemoveClaimAsync(Role, deleteClaim);

            if (!result.Succeeded)
            {
                StatusMessage = "Error: Delete Claim Fail!";
                foreach (var err in result.Errors)
                {
                    ModelState.TryAddModelError(string.Empty,err.Description);
                }
                return Page();
            }

            StatusMessage = "Claim Delete Successful!";

            return RedirectToPage("./CreateOrUpdate", "StartUpdateRole", new { roleID = Role.Id });
        }

    }
}
