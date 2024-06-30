using Microsoft.AspNetCore.Authorization;

namespace RazorWeb.Sercurity.Requirement
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public ArticleUpdateRequirement(int date=1, int month=6, int year=2024)
        {
            Date = date;
            Month = month;
            Year = year;
        }

        public int Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
