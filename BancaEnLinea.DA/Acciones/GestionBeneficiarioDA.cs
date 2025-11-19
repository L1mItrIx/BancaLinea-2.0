using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionBeneficiarioDA : IGestionBeneficiarioDA
    {
      private readonly BancaEnLineaContext bancaEnLineaContext;

    public GestionBeneficiarioDA(BancaEnLineaContext bancaEnLineaContext)
        {
            this.bancaEnLineaContext = bancaEnLineaContext;
        }

     public async Task<bool> registrarBeneficiario(Beneficiarios beneficiario)
   {
  try
      {
        var beneficiarioEntidad = new BeneficiarioDA
       {
          IdCuenta = beneficiario.IdCuenta,
          Alias = beneficiario.Alias,
      Banco = beneficiario.Banco,
       Moneda = beneficiario.Moneda,
       NumeroCuentaDestino = beneficiario.NumeroCuentaDestino,
        Pais = beneficiario.Pais
 };

       bancaEnLineaContext.Beneficiario.Add(beneficiarioEntidad);
      await bancaEnLineaContext.SaveChangesAsync();
    return true;
       }
    catch (Exception)
   {
         return false;
     }
  }

        public async Task<List<Beneficiarios>> obtenerBeneficiariosPorCliente(int idCuenta)
        {
   var beneficiariosDeBaseDeDatos = await bancaEnLineaContext.Beneficiario
   .Where(b => b.IdCuenta == idCuenta)
 .Include(b => b.Cuenta)
              .AsNoTracking()
         .ToListAsync();

       return beneficiariosDeBaseDeDatos.Select(b => new Beneficiarios
    {
      IdBeneficiario = b.IdBeneficiario,
        IdCuenta = b.IdCuenta,
   Alias = b.Alias,
  Banco = b.Banco,
 Moneda = b.Moneda,
   NumeroCuentaDestino = b.NumeroCuentaDestino,
        Pais = b.Pais,
  Cuenta = b.Cuenta != null ? new Cuenta
   {
          Id = b.Cuenta.Id,
        Nombre = b.Cuenta.Nombre,
         PrimerApellido = b.Cuenta.PrimerApellido,
       SegundoApellido = b.Cuenta.SegundoApellido,
Correo = b.Cuenta.Correo,
         Telefono = b.Cuenta.Telefono,
  Rol = b.Cuenta.Rol
      } : null
 }).ToList();
        }

        public async Task<List<Beneficiarios>> obtenerTodosLosBeneficiarios()
 {
       var beneficiariosDeBaseDeDatos = await bancaEnLineaContext.Beneficiario
   .Include(b => b.Cuenta)
  .AsNoTracking()
                .ToListAsync();

   return beneficiariosDeBaseDeDatos.Select(b => new Beneficiarios
  {
                IdBeneficiario = b.IdBeneficiario,
       IdCuenta = b.IdCuenta,
          Alias = b.Alias,
   Banco = b.Banco,
                Moneda = b.Moneda,
     NumeroCuentaDestino = b.NumeroCuentaDestino,
         Pais = b.Pais,
                Cuenta = b.Cuenta != null ? new Cuenta
     {
   Id = b.Cuenta.Id,
        Nombre = b.Cuenta.Nombre,
        PrimerApellido = b.Cuenta.PrimerApellido,
        SegundoApellido = b.Cuenta.SegundoApellido,
          Correo = b.Cuenta.Correo,
Telefono = b.Cuenta.Telefono,
         Rol = b.Cuenta.Rol
       } : null
            }).ToList();
        }

   public async Task<Beneficiarios?> obtenerBeneficiarioPorId(int idBeneficiario)
        {
            var beneficiarioEntidad = await bancaEnLineaContext.Beneficiario
   .Include(b => b.Cuenta)
             .AsNoTracking()
                .FirstOrDefaultAsync(b => b.IdBeneficiario == idBeneficiario);

if (beneficiarioEntidad == null)
         return null;

       return new Beneficiarios
       {
      IdBeneficiario = beneficiarioEntidad.IdBeneficiario,
        IdCuenta = beneficiarioEntidad.IdCuenta,
     Alias = beneficiarioEntidad.Alias,
 Banco = beneficiarioEntidad.Banco,
      Moneda = beneficiarioEntidad.Moneda,
        NumeroCuentaDestino = beneficiarioEntidad.NumeroCuentaDestino,
 Pais = beneficiarioEntidad.Pais,
       Cuenta = beneficiarioEntidad.Cuenta != null ? new Cuenta
    {
         Id = beneficiarioEntidad.Cuenta.Id,
            Nombre = beneficiarioEntidad.Cuenta.Nombre,
       PrimerApellido = beneficiarioEntidad.Cuenta.PrimerApellido,
    SegundoApellido = beneficiarioEntidad.Cuenta.SegundoApellido,
 Correo = beneficiarioEntidad.Cuenta.Correo,
Telefono = beneficiarioEntidad.Cuenta.Telefono,
           Rol = beneficiarioEntidad.Cuenta.Rol
     } : null
  };
      }

    public async Task<bool> actualizarBeneficiario(Beneficiarios beneficiario, int idBeneficiario)
        {
var beneficiarioExistente = await bancaEnLineaContext.Beneficiario.FindAsync(idBeneficiario);
if (beneficiarioExistente == null)
      return false;

    beneficiarioExistente.Alias = beneficiario.Alias;
  beneficiarioExistente.Banco = beneficiario.Banco;
       beneficiarioExistente.Moneda = beneficiario.Moneda;
       beneficiarioExistente.NumeroCuentaDestino = beneficiario.NumeroCuentaDestino;
            beneficiarioExistente.Pais = beneficiario.Pais;
      // IdCuenta no se actualiza

    await bancaEnLineaContext.SaveChangesAsync();
   return true;
   }

      public async Task<bool> eliminarBeneficiario(int idBeneficiario)
   {
            var beneficiario = await bancaEnLineaContext.Beneficiario.FindAsync(idBeneficiario);
     if (beneficiario == null)
       return false;

   bancaEnLineaContext.Beneficiario.Remove(beneficiario);
   await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }
    }
}
