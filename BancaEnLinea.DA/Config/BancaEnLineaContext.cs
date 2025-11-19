using BancaEnLinea.BC.Modelos;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Config
{
  public class BancaEnLineaContext : DbContext
  {
    public BancaEnLineaContext(DbContextOptions<BancaEnLineaContext> options) : base(options) { }

    public DbSet<CuentaDA> Cuenta { get; set; }
    public DbSet<CuentaBancariaDA> CuentaBancaria { get; set; }
    public DbSet<BeneficiarioDA> Beneficiario { get; set; }
    public DbSet<TransferenciaDA> Transferencia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<CuentaBancariaDA>()
            .HasOne(cb => cb.Cuenta)
            .WithMany()
            .HasForeignKey(cb => cb.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<BeneficiarioDA>()
 .HasOne(b => b.Cuenta)
          .WithMany()
      .HasForeignKey(b => b.IdCuenta)
  .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<TransferenciaDA>()
   .HasOne(t => t.CuentaBancariaOrigen)
      .WithMany()
            .HasForeignKey(t => t.IdCuentaBancariaOrigen)
    .OnDelete(DeleteBehavior.Restrict);

     modelBuilder.Entity<TransferenciaDA>()
  .HasOne(t => t.Aprobador)
            .WithMany()
    .HasForeignKey(t => t.IdAprobador)
  .OnDelete(DeleteBehavior.Restrict);

            // Índice único para IdempotencyKey
            modelBuilder.Entity<TransferenciaDA>()
      .HasIndex(t => t.IdempotencyKey)
         .IsUnique();
    }

  }
}
