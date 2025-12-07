using EduVision.Data; // AccountRepository için
using EduVision.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();


// ✅ VERİTABANI BAĞLANTISI (Context = EduDbContext)
builder.Services.AddDbContext<EduDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ... diğer kodlar (AddControllersWithViews vs.) devam eder ...
// ✅ ADO.NET Repository (DI)
builder.Services.AddScoped<AccountRepository>();

// ✅ Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // Giriş yapmayan buraya atılır
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// -------------------- PIPELINE --------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Bunların sırası ÇOK ÖNEMLİ
app.UseAuthentication();
app.UseAuthorization();

// ✅ Varsayılan açılış: Welcome (senin isteğine göre)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Welcome}/{id?}");

app.Run();
