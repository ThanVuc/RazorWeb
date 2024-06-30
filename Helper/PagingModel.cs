namespace RazorWeb.Helper
{
    public class PagingModel
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public Func<int?,string>? GenerateUrl { get; set; }

    }
}
