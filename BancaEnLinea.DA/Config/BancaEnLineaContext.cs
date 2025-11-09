using BancaEnLinea.BC.Modelos;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Config
{
  public class BancaEnLineaContext : DbContext
  {
    public BancaEnLineaContext(DbContextOptions<BancaEnLineaContext> options) : base(options) { }

    public DbSet<Cuenta> Cuenta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }

  }
}
