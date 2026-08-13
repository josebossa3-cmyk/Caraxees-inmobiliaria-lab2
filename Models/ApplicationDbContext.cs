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
    public DbSet<Inquilino> Inquilinos { get; set; }
    public DbSet<Inmueble> Inmuebles { get; set; }
    public DbSet<ImagenInmueble> ImagenesInmueble { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Propietario>().ToTable("propietarios");
      modelBuilder.Entity<Inquilino>().ToTable("inquilino");
      modelBuilder.Entity<Inmueble>().ToTable("inmuebles");
      modelBuilder.Entity<ImagenInmueble>().ToTable("imagenesinmueble");

      modelBuilder.Entity<Propietario>()
        .Property(p => p.DNI)
        .IsRequired()
        .HasMaxLength(20);

      modelBuilder.Entity<Propietario>()
        .HasIndex(p => p.DNI)
        .IsUnique();

      modelBuilder.Entity<Inmueble>()
        .HasOne(i => i.Propietario)
        .WithMany()
        .HasForeignKey(i => i.Propietario)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Inmueble>()
        .HasOne(i => i.TipoInmueble)
        .WithMany()
        .HasForeignKey(i => i.TipoInmuebleId)
        .OnDelete(DeleteBehavior.Restrict);


      modelBuilder.Entity<ImagenInmueble>()
        .HasOne(img => img.Inmueble)
        .WithMany(i => i.ImagenesInmueble)
        .HasForeignKey(img => img.InmuebleId)
        .OnDelete(DeleteBehavior.Cascade);

    }
  }
}