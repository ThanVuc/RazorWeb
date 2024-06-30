using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorWeb.Model;

namespace RazorWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WebBlogContext _blogDBContext;

        public IndexModel(ILogger<IndexModel> logger, WebBlogContext blogDBContext)
        {
            _logger = logger;
            _blogDBContext = blogDBContext;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public void OnGet()
        {
            var post = (from a in _blogDBContext.Articles
                            orderby a.Created descending
                            select a).ToList();
            ViewData["post"] = post;
        }
    }
}
