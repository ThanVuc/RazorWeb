using App.Admin.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace App.Admin.Role
{
    [Authorize(Policy = "AdministratorPermission")]
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(webBlogContext, logger, roleManager, userManager)
        {
        }

        public class InputModel
        {
            public string? ID { get; set; }
            [Required(ErrorMessage = "Name is Require")]
            public string? Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet()
        {
            return Content("Not Found");
        }

        public IActionResult OnPost()
        {
            return Content("Not Found");
        }

        public async Task<IActionResult> OnPostStartDelete()
        {
            if (Input.ID == null)
            {
                StatusMessage = "Error: Don't Take Infomation Of ID";
                return Page();
            }

            var roleDelele = await _roleManager.FindByIdAsync(Input.ID);
            Input.Name = roleDelele.Name;

            StatusMessage = $"Do you want to Delete {Input.Name}?";
            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = null;
                return Page();
            }

            var roleDeleter = await _roleManager.FindByIdAsync(Input.ID);
            if (roleDeleter != null)
            {
                var result = await _roleManager.DeleteAsync(roleDeleter);
                if (result.Succeeded)
                {
                    StatusMessage = "Delete Successful!";
                    return RedirectToPage("./Index");
                } else
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.TryAddModelError(string.Empty,err.Description);
                    }
                }
            } else
            {
                StatusMessage = "Error: Don't Take Infomation Of ID";
                return Page();
            }
            return Page();
        }

    }
}
