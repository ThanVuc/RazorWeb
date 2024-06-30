using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RazorWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace App.Admin.Role
{
    [Authorize(Policy = "AdministratorPermission")]
    public class CreateModel : RolePageModel
    {
        public CreateModel(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(webBlogContext, logger, roleManager, userManager)
        {
        }

        public IActionResult OnGet() { return Content("Not Found Get"); }
        public IActionResult OnPost() { return Content("Not Found Post"); }

        public class InputModel
        {
            public string? ID { set; get; }

            [Required(ErrorMessage = "Require Fill Role Name")]
            [Display(Name = "Role Name")]
            [StringLength(100, ErrorMessage = "{0} From {2} To {1} Char.", MinimumLength = 3)]
            public string? Name { set; get; }

        }

        public IList<IdentityRoleClaim<string>> Claims { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public bool IsUpdate { get; set; }

        public string btnText { get; set; }

        public IActionResult OnPostStartCreateRole()
        {
            StatusMessage = "Let's Fill Infomation To Create New Role";
            btnText = "Create";
            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnGetStartUpdateRole(string? roleID)
        {
            Input = new InputModel();

            if (!roleID.IsNullOrEmpty())
            {
                Input.ID = roleID;
            }

            IsUpdate = false;
            btnText = "Update";

            if (roleID == null)
            {
                StatusMessage = "Error: Not Found Role Infomation";
                ModelState.Clear();
                return Page();
            }

            var result = await _roleManager.FindByIdAsync(roleID);
            if (result != null)
            {
                Claims = await _context.RoleClaims.Where(r => r.RoleId == result.Id).ToListAsync();

                Input.Name = result.Name;

                IsUpdate = true;
                btnText = "Update";
                ViewData["Title"] = "Update Role: " + Input.Name;
            }
            else
            {
                StatusMessage = "Error: Not Found Role ID Infomation";
            }
            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPostStartUpdateRole()
        {
            StatusMessage = null;
            IsUpdate = false;
            btnText = "Update";

            if (Input.ID == null)
            {
                StatusMessage = "Error: Not Found Role Infomation";
                ModelState.Clear();
                return Page();
            }

            var result = await _roleManager.FindByIdAsync(Input.ID);
            if (result != null)
            {
                Claims = await _context.RoleClaims.Where(r => r.RoleId == result.Id).ToListAsync();
                

                Input.Name = result.Name;
                IsUpdate = true;
                btnText = "Update";
                ViewData["Title"] = "Update Role: " + Input.Name;
            } else
            {
                StatusMessage = "Error: Not Found Role ID Infomation";
            }
            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPostAddOrUpdate()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = null;
                return Page();
            }

            if (Input.Name == null)
            {
                StatusMessage = "Error: Don't Have infomation of Role";
                return Page();
            }

            if (IsUpdate)
            {
                var updateRole = await _roleManager.FindByIdAsync(Input.ID);
                if (updateRole != null)
                {
                    updateRole.Name = Input.Name;
                    var result = await _roleManager.UpdateAsync(updateRole);
                    if (result.Succeeded)
                    {
                        StatusMessage = "Update Successful! " + updateRole.Name;
                        return RedirectToPage("./index");
                    } else
                    {
                        StatusMessage = "Error: ";
                        foreach (var err in result.Errors)
                        {
                            StatusMessage += err.Description;
                        }
                    }
                } else
                {
                    StatusMessage = "Error: Not Found this Role ID";
                }
            } else
            {
                var newRole = new IdentityRole(Input.Name);

                var addResult = await _roleManager.CreateAsync(newRole);

                if (addResult.Succeeded)
                {
                    StatusMessage = "Create New Role Success";
                    return RedirectToPage("./index");
                } else
                {
                    StatusMessage = "Error: ";
                    foreach (var err in addResult.Errors)
                    {
                        StatusMessage += err.Description;
                    }
                }
            }
            return Page();
        }

    }
}
