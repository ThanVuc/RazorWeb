using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorWeb.Model;

namespace RazorWeb.Pages_Blog
{
    [Authorize(Policy = "UserControl")]
    public class EditModel : PageModel
    {
        private readonly WebBlogContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(WebBlogContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return Content("Không Tìm Thấy Bài Viết");
            }

            var article =  await _context.Articles.FirstOrDefaultAsync(m => m.Artical_ID == id);
            if (article == null)
            {
                return Content("Không Tìm Thấy Bài Viết");
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var authorizeResult = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");

            if (!authorizeResult.Succeeded)
            {
                return Content("Expired For Update!");
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Artical_ID))
                {
                    return Content("Không Tìm Thấy Bài Viết");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Artical_ID == id);
        }
    }
}
