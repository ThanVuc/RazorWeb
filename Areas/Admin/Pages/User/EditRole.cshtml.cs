using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RazorWeb.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace App.Admin.User
{
    [Authorize(Policy = "AdministratorPermission")]
    public class EditRole : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<EditRole> _logged;
        private readonly WebBlogContext _context;


        public EditRole(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<EditRole> logged, WebBlogContext blogContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logged = logged;
            _context = blogContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        [DisplayName("ListRoles")]
        public string[] ListRoles { get; set; }

        //public SelectList AllRoles { get; set; }

        public string[] AllRoles { get; set; }


        public AppUser user { get; set; }

        public IList<IdentityRoleClaim<string>> RoleClaims { get; set; }
        public IList<IdentityUserClaim<string>> UserClaims { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return Content("Not Exists User");
            }

            user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return Content("Not Exists User");
            }

            ListRoles = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            AllRoles = await _roleManager.Roles.Select(r => r.Name).ToArrayAsync<string>();

            var qr = from rc in _context.RoleClaims
                     join ur in _context.UserRoles on rc.RoleId equals ur.RoleId
                     where ur.UserId == user.Id
                     select rc;
            RoleClaims = await qr.ToListAsync();

            UserClaims = await _context.UserClaims.Where(uc => uc.UserId == user.Id).ToListAsync();


            return Page();
        }

        public async Task<IActionResult> OnPost(string id) 
        {
            if (id.IsNullOrEmpty())
            {
                return Content("Not Found User");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Content("Not Found User");
            }

            var oldRole = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            var deleted = oldRole.Where(r => !ListRoles.Contains(r)).ToArray<string>();

            var inserted = ListRoles.Where(r => !oldRole.Contains(r)).ToArray<string>();

            //List<string> listAllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = await _roleManager.Roles.Select(r => r.Name).ToArrayAsync<string>();

            var DeleteResult = await _userManager.RemoveFromRolesAsync(user, deleted);
            if (!DeleteResult.Succeeded)
            {
                foreach (var err in DeleteResult.Errors)
                {
                    ModelState.AddModelError(string.Empty,err.Description);
                }
            }

            var InsertResult = await _userManager.AddToRolesAsync(user, inserted);
            if (!InsertResult.Succeeded)
            {
                foreach (var err in InsertResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }

            StatusMessage = $"Just Have Update Role For: {user.UserName}";

            return RedirectToPage("./index");
        }    

    }
}
