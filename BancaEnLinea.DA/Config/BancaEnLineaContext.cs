using BancaEnLinea.BC.Modelos;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Config
{
  public class BancaEnLineaContext : DbContext
  {
    public BancaEnLineaContext(DbContextOptions<BancaEnLineaContext> options) : base(options) { }

    public DbSet<Cuenta> Cuenta { get; set; }
    public DbSet<CuentaBancariaDA> CuentaBancaria { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<CuentaBancariaDA>()
            .HasOne(cb => cb.Cuenta)
            .WithMany()
            .HasForeignKey(cb => cb.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);
    }

  }
}
