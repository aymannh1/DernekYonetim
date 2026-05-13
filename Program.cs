using DernekYonetim.Data;
using DernekYonetim.Models;
using DernekYonetim.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════
// 1. VERİTABANI
// ══════════════════════════════════════════════════════

// Convert postgres:// URL (Render format) to Npgsql key=value format
static string ResolveConnectionString(string? raw)
{
    if (string.IsNullOrEmpty(raw)) return "";
    if (!raw.StartsWith("postgres://") && !raw.StartsWith("postgresql://")) return raw;
    var uri = new Uri(raw);
    var parts = uri.UserInfo.Split(':', 2);
    var port = uri.Port == -1 ? 5432 : uri.Port;
    return $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};" +
           $"Username={parts[0]};Password={Uri.UnescapeDataString(parts[1])};" +
           $"SSL Mode=Require;Trust Server Certificate=true";
}

var connectionString = ResolveConnectionString(
    builder.Configuration.GetConnectionString("DefaultConnection"));

// Use NpgsqlDataSourceBuilder so legacy timestamp behavior applies to all DateTime parameters
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableLegacyTimestampBehavior();
var npgsqlDataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(npgsqlDataSource, sql => sql.EnableRetryOnFailure(3)));

// ══════════════════════════════════════════════════════
// 2. IDENTITY
// ══════════════════════════════════════════════════════
builder.Services.AddIdentity<Uye, IdentityRole>(opts =>
{
    // Şifre kuralları
    opts.Password.RequiredLength = 8;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireDigit = true;
    opts.Password.RequireNonAlphanumeric = true;

    // Hesap kilitleme – 5 başarısız denemede 15 dk
    opts.Lockout.MaxFailedAccessAttempts = 5;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    opts.Lockout.AllowedForNewUsers = true;

    opts.User.RequireUniqueEmail = true;
    opts.SignIn.RequireConfirmedEmail = false; // Geliştirme: false
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ══════════════════════════════════════════════════════
// 3. COOKIE AYARLARI
// ══════════════════════════════════════════════════════
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Auth/Login";
    opts.LogoutPath = "/Auth/Logout";
    opts.AccessDeniedPath = "/Auth/AccessDenied";
    opts.ExpireTimeSpan = TimeSpan.FromHours(8);
    opts.SlidingExpiration = true;
    opts.Cookie.HttpOnly = true;
    opts.Cookie.SameSite = SameSiteMode.Lax;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ══════════════════════════════════════════════════════
// 4. MVC
// ══════════════════════════════════════════════════════
builder.Services.AddControllersWithViews();

// ══════════════════════════════════════════════════════
// 5. UYGULAMA SERVİSLERİ
// ══════════════════════════════════════════════════════
builder.Services.AddScoped<UyeService>();
builder.Services.AddScoped<AidatService>();
builder.Services.AddScoped<EtkinlikService>();
builder.Services.AddScoped<BildirimService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<EmailService>();

// ══════════════════════════════════════════════════════
// 6. SESSION (flash mesajlar)
// ══════════════════════════════════════════════════════
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.IsEssential = true;
});

// ══════════════════════════════════════════════════════
// 7. ANTI-FORGERY (CSRF)
// ══════════════════════════════════════════════════════
builder.Services.AddAntiforgery(opts => opts.HeaderName = "X-CSRF-TOKEN");

// ══════════════════════════════════════════════════════
var app = builder.Build();
// ══════════════════════════════════════════════════════

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ══════════════════════════════════════════════════════
// 8. ROUTE'LAR
// ══════════════════════════════════════════════════════
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ══════════════════════════════════════════════════════
// 9. VERİTABANI SEED
// ══════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider;
    try
    {
        var db = svc.GetRequiredService<AppDbContext>();
        var userManager = svc.GetRequiredService<UserManager<Uye>>();
        var roleManager = svc.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var log = svc.GetRequiredService<ILogger<Program>>();
        log.LogError(ex, "Veritabanı seed hatası");
    }
}

app.Run(); 