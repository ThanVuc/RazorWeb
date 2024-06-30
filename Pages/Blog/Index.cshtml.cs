using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorWeb.Model;

namespace RazorWeb.Pages_Blog
{
    [Authorize(Policy = "IsGenZ")]
    public class IndexModel : PageModel
    {
        private readonly RazorWeb.Model.WebBlogContext _context;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; } = 10;
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(RazorWeb.Model.WebBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;

        public async Task OnGetAsync(string SerchString)
        {
            TotalItem = _context.Articles.Count();
            TotalPage = (int)Math.Ceiling((double)TotalItem / ItemPerPage);

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPage) CurrentPage = TotalPage;

            var qr = (from a in _context.Articles
                      orderby a.Created descending
                      select a);
            Article = await qr.ToListAsync();

            if (SerchString != null)
            {
                Article = await qr.Where(a => a.Title.Contains(SerchString)).ToListAsync();
            }
            else
            {
                Article = await qr.Skip((CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToListAsync();
            }
            

        }

    }
}
