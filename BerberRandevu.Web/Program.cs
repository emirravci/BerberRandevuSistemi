using AutoMapper;
using BerberRandevu.Application.Arayuzler.BirimIs;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Application.Arayuzler.Servisler;
using BerberRandevu.Application.Profiller;
using BerberRandevu.Application.Servisler;
using BerberRandevu.Domain.Kullanicilar;
using BerberRandevu.Infrastructure.BirimIs;
using BerberRandevu.Infrastructure.Depolar;
using BerberRandevu.Infrastructure.Kimlik;
using BerberRandevu.Infrastructure.VeriErisim;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext + SQL Server
var connectionString = builder.Configuration.GetConnectionString("VarsayilanBaglanti")
                       ?? throw new InvalidOperationException("VarsayilanBaglanti connection string'i bulunamadı.");

builder.Services.AddDbContext<BerberDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services
    .AddIdentity<UygulamaKullanicisi, IdentityRole>()
    .AddEntityFrameworkStores<BerberDbContext>()
    .AddDefaultTokenProviders();

// Uygulama servisleri ve repository DI kayıtları
builder.Services.AddScoped<IRandevuServisi, RandevuServisi>();
builder.Services.AddScoped<IPersonelServisi, PersonelServisi>();
builder.Services.AddScoped<IOdemeServisi, OdemeServisi>();
builder.Services.AddScoped<IGiderServisi, GiderServisi>();
builder.Services.AddScoped<IHizmetServisi, HizmetServisi>();
builder.Services.AddScoped<ISalonAyarlariServisi, SalonAyarlariServisi>();

builder.Services.AddScoped<IRandevuRepository, RandevuRepository>();
builder.Services.AddScoped<IPersonelRepository, PersonelRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(GenelProfil).Assembly);

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed (roller, admin, örnek personel)
await AltyapiBaslatma.BaslatAsync(app.Services);

// HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hesap}/{action=Giris}/{id?}");

app.Run();
