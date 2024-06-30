using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RazorWeb.Model;
namespace App.Admin.User
{
    [Authorize(Policy = "AdministratorPermission")]
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; } = 10;
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }

        private readonly UserManager<AppUser> _userManager;
        private readonly WebBlogContext _context;

        public IndexModel(UserManager<AppUser> userManager, WebBlogContext webBlogContext)
        {
            _userManager = userManager;
            _context = webBlogContext;
        }

        public class UserAndRole : AppUser
        {
            public string RolesName { get; set; }
        }

        public IList<UserAndRole> Users { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet()
        {
            var qr = _userManager.Users;

            TotalItem = await qr.CountAsync();
            TotalPage = (int)Math.Ceiling((double)TotalItem / ItemPerPage);

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPage) CurrentPage = TotalPage;

            var qr1 = qr.Skip((CurrentPage - 1) * ItemPerPage).Take(ItemPerPage)
                .Select(u => new UserAndRole()
                {
                    Id = u.Id,
                    UserName = u.UserName
                });

            Users = await qr1.ToListAsync();

            foreach (var user in Users)
            {
                string[] roles = (await _userManager.GetRolesAsync(user)).ToArray<string>();
                user.RolesName = String.Join(",",roles);
            }

        }

    }
}
