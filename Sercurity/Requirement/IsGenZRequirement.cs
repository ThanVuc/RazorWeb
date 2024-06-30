using Microsoft.AspNetCore.Authorization;

namespace RazorWeb.Sercurity.Requirement
{
    public class IsGenZRequirement : IAuthorizationRequirement
    {
        public IsGenZRequirement(int fromYear = 1997, int toYear = 2012)
        {
            FromYear = fromYear;
            ToYear = toYear;
        }
        public int FromYear { get; set; }
        public int ToYear { get; set; }
    }
}
