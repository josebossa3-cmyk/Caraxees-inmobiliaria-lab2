using Microsoft.EntityFrameworkCore;

namespace inmobiliaria.Models
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }

    public DbSet<Propietario> Propietarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Propietario>().ToTable("propietarios");

      modelBuilder.Entity<Propietario>()
        .Property(p => p.DNI)
        .IsRequired()
        .HasMaxLength(20);
      modelBuilder.Entity<Propietario>()
        .HasIndex(p => p.DNI)
        .IsUnique();
    }
  }
}