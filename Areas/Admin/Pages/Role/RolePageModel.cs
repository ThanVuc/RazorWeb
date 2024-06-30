using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorWeb.Model;

namespace App.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly WebBlogContext _context;
        protected readonly ILogger<RolePageModel> _logger;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly UserManager<AppUser> _userManager;


        [TempData]
        public string? StatusMessage { get; set; }

        public RolePageModel(WebBlogContext webBlogContext, ILogger<RolePageModel> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _context = webBlogContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }
    }
}
