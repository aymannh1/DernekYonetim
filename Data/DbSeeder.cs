using DernekYonetim.Models;
using Microsoft.AspNetCore.Identity;

namespace DernekYonetim.Data
{
    public static class DbSeeder
    {
        public const string RolAdmin = "Admin";
        public const string RolYonetim = "Yonetim";
        public const string RolUye = "Uye";

        public static async Task SeedAsync(
            UserManager<Uye> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 1 ── Rolleri oluştur
            foreach (var rol in new[] { RolAdmin, RolYonetim, RolUye })
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }

            // 2 ── Varsayılan admin
            const string adminEmail = "admin@dernekdbs.org";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new Uye
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Ad = "Sistem",
                    Soyad = "Yöneticisi",
                    UyeNo = "ADM-0001",
                    Telefon = "05000000000",
                    UyelikDurumu = UyelikDurumu.Aktif
                };

                // ⚠️  İlk girişten sonra değiştirin!
                var result = await userManager.CreateAsync(admin, "Admin@12345!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, RolAdmin);
            }
        }
    }
}