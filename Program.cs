using CountryMaps.TerminalsLoader.Data;
using CountryMaps.TerminalsLoader.Hosted;
using CountryMaps.TerminalsLoader.Repositories;
using CountryMaps.TerminalsLoader.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

//Поддержка кирилицы в консоли
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOfficesImportService, OfficesImportService>();
builder.Services.AddScoped<ICountryMapsRepository, CountryMapsRepository>();
builder.Services.AddHostedService<OfficesImportBackgroundService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

await host.RunAsync();

