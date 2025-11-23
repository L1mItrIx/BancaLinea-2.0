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
    public DbSet<ServicioDA> Servicio { get; set; }
    public DbSet<ContratoServicioDA> ContratoServicio { get; set; }
    public DbSet<PagoServicioDA> PagoServicio { get; set; }
    public DbSet<AccionDA> Accion { get; set; }  // ?? Nueva tabla

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

      modelBuilder.Entity<ContratoServicioDA>()
    .HasOne(cs => cs.Servicio)
   .WithMany()
        .HasForeignKey(cs => cs.IdServicio)
  .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<ContratoServicioDA>()
   .HasIndex(cs => cs.NumeroContrato)
 .IsUnique();

modelBuilder.Entity<PagoServicioDA>()
        .HasOne(ps => ps.ContratoServicio)
   .WithMany()
.HasForeignKey(ps => ps.IdContratoServicio)
 .OnDelete(DeleteBehavior.Restrict);

     modelBuilder.Entity<PagoServicioDA>()
        .HasOne(ps => ps.CuentaBancariaOrigen)
.WithMany()
   .HasForeignKey(ps => ps.IdCuentaBancariaOrigen)
  .OnDelete(DeleteBehavior.Restrict);
    }

  }
}
