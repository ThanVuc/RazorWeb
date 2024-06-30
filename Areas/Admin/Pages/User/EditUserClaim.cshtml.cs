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
    public class EditUserClaim : RolePageModel
    {
        public EditUserClaim(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(webBlogContext, logger, roleManager, userManager)
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
        public int claimID { get; set; }
        public IdentityUserClaim<string> Claim { get; set; }
        public AppUser user { get; set; }
        public bool IsAdd { get; set; } = false;

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Claim = (_context.UserClaims.Where(c => c.Id == claimID)).FirstOrDefault();
            if (Claim == null)
            {
                return Content("Not Found Claim");
            }

            user = await _userManager.FindByIdAsync(Claim.UserId);

            if (user == null)
            {
                return Content("Not Found User");
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
            _logger.LogInformation(claimID.ToString());
            Claim = (_context.UserClaims.Where(c => c.Id == claimID)).FirstOrDefault();
            
            if (Claim == null)
            {
                return Content("Not Found Claim");
            }

            user = await _userManager.FindByIdAsync(Claim.UserId);

            if (user == null)
            {
                return Content("Not Found User");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.UserClaims.Any(uc => uc.UserId == user.Id && uc.ClaimType == Input.ClaimType && uc.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim already exists!");
                return Page();
            }

            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage = "Update Successful!";

            return RedirectToPage("./EditRole",new {id = user.Id});

        }

        public async Task<IActionResult> OnPostDeleteClaims()
        {
            _logger.LogInformation(claimID.ToString());

            Claim = (_context.UserClaims.Where(c => c.Id == claimID)).FirstOrDefault();

            if (Claim == null)
            {
                return Content("Not Found Claim");
            }

            user = await _userManager.FindByIdAsync(Claim.UserId);

            if (user == null)
            {
                return Content("Not Found User");
            }
            var deleteClaim = new Claim(Claim.ClaimType, Claim.ClaimValue);
            await _userManager.RemoveClaimAsync(user, deleteClaim);

            await _context.SaveChangesAsync();
            StatusMessage = "Delete Successful!";

            return RedirectToPage("./EditRole", new { id = user.Id });
        }

        public async Task<IActionResult> OnGetAddClaim(string id)
        {
            IsAdd = true;
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Content("Not Found User!");
            }

            StatusMessage = "Fill Aprepriate Infomation To Add User Claim";

            return Page();
        }

        public async Task<IActionResult> OnPostAddClaim(string id)
        {
            if (id == null)
            {
                return Content("Not Found ID!");
            }

            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Content("Not Found User");
            }

            Claim addClaim = new Claim(Input.ClaimType, Input.ClaimValue);

            if (_context.UserClaims.Any(uc => uc.UserId == user.Id && uc.ClaimType == Input.ClaimType && uc.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim already exists!");
                return Page();
            }

            var result = await _userManager.AddClaimAsync(user, addClaim);

            if (!result.Succeeded)
            {
                StatusMessage = "Error: Add New Claim Fail";
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }
                return Page();
            }

            StatusMessage = "Add New Claim Successful!";
            return RedirectToPage("./EditRole", new { id = user.Id });
        }

    }
}
