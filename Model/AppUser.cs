using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RazorWeb.Model
{
    public class AppUser : IdentityUser
    {
        [DataType("Nvarchar(200)")]
        [PersonalData]
        public string? Address { get; set; }

        [DataType("Date")]
        [PersonalData]
        public DateOnly? Birth { get; set; }

    }
}
