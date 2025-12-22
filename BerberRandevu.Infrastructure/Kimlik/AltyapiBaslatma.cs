using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Domain.Varliklar;
using BerberRandevu.Infrastructure.VeriErisim;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BerberRandevu.Infrastructure.Kimlik;

/// <summary>
/// Uygulama ilk çalıştığında rollerin, admin hesabının ve örnek personelin oluşturulmasını sağlar.
/// </summary>
public static class AltyapiBaslatma
{
    private static readonly string[] Roller = { "Admin", "Personel", "Kullanıcı" };

    public static async Task BaslatAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var context = scopedProvider.GetRequiredService<BerberDbContext>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scopedProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();

        await context.Database.MigrateAsync();

        // Roller
        foreach (var rolAdi in Roller)
        {
            if (!await roleManager.RoleExistsAsync(rolAdi))
            {
                await roleManager.CreateAsync(new IdentityRole(rolAdi));
            }
        }

        // Varsayılan admin
        const string adminEmail = "admin@berber.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new UygulamaKullanicisi
            {
                UserName = adminEmail,
                Email = adminEmail,
                AdSoyad = "Sistem Yöneticisi"
            };

            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Örnek personel
        const string personelEmail = "personel@berber.local";
        var personelKullanici = await userManager.FindByEmailAsync(personelEmail);
        if (personelKullanici == null)
        {
            personelKullanici = new UygulamaKullanicisi
            {
                UserName = personelEmail,
                Email = personelEmail,
                AdSoyad = "Örnek Personel"
            };

            await userManager.CreateAsync(personelKullanici, "Personel123!");
            await userManager.AddToRoleAsync(personelKullanici, "Personel");
        }

        // Personel tablosunda yoksa ekle
        if (!await context.Personeller.AnyAsync())
        {
            var personel = new Personel
            {
                KullaniciId = personelKullanici.Id,
                Ad = "Örnek",
                Soyad = "Personel",
                AktifMi = true
            };

            context.Personeller.Add(personel);
            await context.SaveChangesAsync();
        }
    }
}


