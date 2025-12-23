using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Domain.Varliklar;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Infrastructure.VeriErisim;

/// <summary>
/// Uygulamanın ana veritabanı bağlamı.
/// Identity ve domain varlıklarını birlikte yönetir.
/// </summary>
public class BerberDbContext : IdentityDbContext<UygulamaKullanicisi>
{
    public BerberDbContext(DbContextOptions<BerberDbContext> options) : base(options)
    {
    }

    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<CalismaSaati> CalismaSaatleri => Set<CalismaSaati>();
    public DbSet<Randevu> Randevular => Set<Randevu>();
    public DbSet<Odeme> Odemeler => Set<Odeme>();
    public DbSet<Gider> Giderler => Set<Gider>();
    public DbSet<Hizmet> Hizmetler => Set<Hizmet>();
    public DbSet<SalonAyarlari> SalonAyarlari => Set<SalonAyarlari>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Basit konfigürasyonlar: zorunlu alanlar vb.
        builder.Entity<Personel>(b =>
        {
            b.Property(x => x.Ad).IsRequired().HasMaxLength(100);
            b.Property(x => x.Soyad).IsRequired().HasMaxLength(100);
        });

        builder.Entity<Randevu>(b =>
        {
            b.HasOne(r => r.Personel)
                .WithMany(p => p.Randevular)
                .HasForeignKey(r => r.PersonelId);

            b.HasOne(r => r.Odeme)
                .WithOne(o => o.Randevu)
                .HasForeignKey<Odeme>(o => o.RandevuId);
        });

        builder.Entity<CalismaSaati>(b =>
        {
            b.HasOne(cs => cs.Personel)
                .WithMany(p => p.CalismaSaatleri)
                .HasForeignKey(cs => cs.PersonelId);
        });

        builder.Entity<Hizmet>(b =>
        {
            b.Property(x => x.Ad).IsRequired().HasMaxLength(100);
            b.Property(x => x.Sure).IsRequired();
            b.Property(x => x.Ucret).HasColumnType("decimal(18,2)").IsRequired();
        });

        builder.Entity<SalonAyarlari>(b =>
        {
            b.Property(x => x.Anahtar).IsRequired().HasMaxLength(100);
            b.Property(x => x.Deger).IsRequired().HasMaxLength(500);
            b.HasIndex(x => x.Anahtar).IsUnique();
        });
    }
}


