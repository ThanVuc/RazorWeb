using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using RazorWeb.Model;
using RazorWeb.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using RazorWeb.Sercurity.Requirement;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WebBlogContext") ?? throw new InvalidOperationException("Connection string 'WebBlogContextConnection' not found.");

var services = builder.Services;
var configuration = builder.Configuration;
services.AddDbContext<WebBlogContext>(options =>
{
    var connectString = configuration.GetConnectionString("WebBlogContext");

    options.UseSqlServer(connectString);
});


services.AddOptions();

services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<WebBlogContext>()
    .AddDefaultTokenProviders();

//services.AddDefaultIdentity<AppUser>()
//    .AddEntityFrameworkStores<WebBlogContext>()
//    .AddDefaultTokenProviders();




services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;
});

services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/";
    options.LogoutPath = "/Logout/";
    options.AccessDeniedPath = "/AccessDenied/";
});

services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleConfig = configuration.GetSection("Authentication:Google");
        options.ClientId = googleConfig["ClientID"];
        options.ClientSecret = googleConfig["ClientSecret"];
        //http://localhost:5214/google-login
        options.CallbackPath = "/google-login";
    })
    .AddFacebook(options =>
    {
        var facebookConfig = configuration.GetSection("Authentication:Facebook");
        options.ClientId = facebookConfig["ClientID"];
        options.ClientSecret = facebookConfig["ClientSecret"];
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("AdministratorPermission", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();

        policyBuilder.RequireClaim("CanRead", "Admin");
        policyBuilder.RequireClaim("CanEdit", "Admin");
        policyBuilder.RequireClaim("CanDelete", "Admin");
    });

    options.AddPolicy("UserControl", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();

        policyBuilder.RequireClaim("CanRead", "User");
        policyBuilder.RequireClaim("CanEdit", "User");
        policyBuilder.RequireClaim("CanDelete", "User");

    });

    options.AddPolicy("DesignerControl", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();

        policyBuilder.RequireClaim("CanRead", "Designer");
        policyBuilder.RequireClaim("CanEdit", "Designer");

    });

    options.AddPolicy("IsGenZ", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new IsGenZRequirement());
    });

    options.AddPolicy("ShowAdminMenu", policyBuilder =>
    {
        policyBuilder.RequireRole("Admin");
    });

    options.AddPolicy("CanUpdateArticle", policy =>
    {
        policy.Requirements.Add(new ArticleUpdateRequirement());
    });

});

var mailSettings = configuration.GetSection("MailSettings");
services.Configure<MailSettings>(mailSettings);
services.AddTransient<IEmailSender, SendMailService>();
services.AddTransient<IAuthorizationHandler,AppAuthorizationHandler>();


// Add services to the container.
services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

