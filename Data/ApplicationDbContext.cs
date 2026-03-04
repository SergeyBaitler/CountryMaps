using CountryMaps.TerminalsLoader.Entities;
using Microsoft.EntityFrameworkCore;

namespace CountryMaps.TerminalsLoader.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Office> Offices => Set<Office>();
    public DbSet<Phone> Phones => Set<Phone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var office = modelBuilder.Entity<Office>();

        office.ToTable("offices");
        office.HasKey(o => o.Id);

        office.Property(o => o.Id).HasColumnName("id");
        office.Property(o => o.Code).HasColumnName("code").HasMaxLength(64);
        office.Property(o => o.CityCode).HasColumnName("city_code");
        office.Property(o => o.Uuid).HasColumnName("uuid").HasMaxLength(64);
        office.Property(o => o.Type).HasColumnName("type");
        office.Property(o => o.CountryCode).HasColumnName("country_code").HasMaxLength(8);
        office.Property(o => o.WorkTime).HasColumnName("work_time").HasMaxLength(256);

        office.OwnsOne(o => o.Coordinates, coord =>
        {
            coord.Property(c => c.Latitude).HasColumnName("latitude");
            coord.Property(c => c.Longitude).HasColumnName("longitude");
        });

        office.OwnsOne(o => o.Address, addr =>
        {
            addr.Property(a => a.Country).HasColumnName("addr_country").HasMaxLength(128);
            addr.Property(a => a.City).HasColumnName("addr_city").HasMaxLength(128);
            addr.Property(a => a.Street).HasColumnName("addr_street").HasMaxLength(256);
            addr.Property(a => a.House).HasColumnName("addr_house").HasMaxLength(32);
        });

        office.HasIndex(o => o.Uuid).IsUnique();
        office.HasIndex(o => o.Code);
        office.HasIndex(o => new { o.CountryCode, o.CityCode });

        var phone = modelBuilder.Entity<Phone>();

        phone.ToTable("office_phones");
        phone.HasKey(p => p.Id);
        phone.Property(p => p.Id).HasColumnName("id");
        phone.Property(p => p.Number).HasColumnName("number").HasMaxLength(32);
        phone.Property(p => p.Comment).HasColumnName("comment").HasMaxLength(128);

        phone.HasOne<Office>()
            .WithMany(o => o.Phones)
            .HasForeignKey("office_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

