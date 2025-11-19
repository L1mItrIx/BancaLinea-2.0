using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
 public class GestionBeneficiarioBW : IGestionBeneficiarioBW
    {
        private readonly IGestionBeneficiarioDA gestionBeneficiarioDA;
        private readonly IGestionCuentaDA gestionCuentaDA;

      public GestionBeneficiarioBW(IGestionBeneficiarioDA gestionBeneficiarioDA, IGestionCuentaDA gestionCuentaDA)
        {
    this.gestionBeneficiarioDA = gestionBeneficiarioDA;
            this.gestionCuentaDA = gestionCuentaDA;
      }

        public async Task<bool> registrarBeneficiario(Beneficiarios beneficiario)
   {
 // Validar que el beneficiario sea válido
   if (!ReglasDeBeneficiario.elBeneficiarioEsValido(beneficiario))
  {
          return false;
    }

      // Verificar que el cliente (cuenta) exista
            var cliente = await gestionCuentaDA.obtenerCuentaPorId(beneficiario.IdCuenta);
  if (cliente == null)
       {
         return false;
   }

   // Obtener beneficiarios existentes del cliente
     var beneficiariosExistentes = await gestionBeneficiarioDA.obtenerBeneficiariosPorCliente(beneficiario.IdCuenta);

// Validar que no exceda el límite de 3 beneficiarios por cliente
            if (!ReglasDeBeneficiario.puedeAgregarBeneficiario(beneficiariosExistentes, beneficiario.IdCuenta))
      {
     return false;
            }

      // Validar que el alias sea único para este cliente
    if (!ReglasDeBeneficiario.elAliasEsUnico(beneficiariosExistentes, beneficiario.Alias, beneficiario.IdCuenta))
       {
      return false;
 }

 // Validar que el número de cuenta destino sea único para este cliente
            if (!ReglasDeBeneficiario.elNumeroCuentaDestinoEsUnico(beneficiariosExistentes, beneficiario.NumeroCuentaDestino, beneficiario.IdCuenta))
   {
 return false;
            }

    return await gestionBeneficiarioDA.registrarBeneficiario(beneficiario);
     }

 public Task<List<Beneficiarios>> obtenerBeneficiariosPorCliente(int idCuenta)
        {
 return ReglasDeBeneficiario.elIdEsValido(idCuenta) ?
       gestionBeneficiarioDA.obtenerBeneficiariosPorCliente(idCuenta) :
        Task.FromResult(new List<Beneficiarios>());
   }

        public Task<List<Beneficiarios>> obtenerTodosLosBeneficiarios()
    {
      return gestionBeneficiarioDA.obtenerTodosLosBeneficiarios();
        }

        public Task<Beneficiarios?> obtenerBeneficiarioPorId(int idBeneficiario)
     {
            return ReglasDeBeneficiario.elIdEsValido(idBeneficiario) ?
    gestionBeneficiarioDA.obtenerBeneficiarioPorId(idBeneficiario) :
      Task.FromResult<Beneficiarios?>(null);
        }

        public async Task<bool> actualizarBeneficiario(Beneficiarios beneficiario, int idBeneficiario)
        {
   if (!ReglasDeBeneficiario.elIdEsValido(idBeneficiario))
    {
     return false;
    }

// Obtener el beneficiario existente
          var beneficiarioExistente = await gestionBeneficiarioDA.obtenerBeneficiarioPorId(idBeneficiario);
         if (beneficiarioExistente == null)
  {
     return false;
  }

      // Preservar el IdCuenta original
    beneficiario.IdCuenta = beneficiarioExistente.IdCuenta;

            // Validar el beneficiario
   if (!ReglasDeBeneficiario.elBeneficiarioEsValido(beneficiario))
   {
  return false;
  }

  // Obtener beneficiarios existentes del cliente
     var beneficiariosExistentes = await gestionBeneficiarioDA.obtenerBeneficiariosPorCliente(beneficiario.IdCuenta);

      // Validar que el alias sea único (excluyendo el beneficiario actual)
       if (!ReglasDeBeneficiario.elAliasEsUnico(beneficiariosExistentes, beneficiario.Alias, beneficiario.IdCuenta, idBeneficiario))
            {
     return false;
       }

       // Validar que el número de cuenta destino sea único (excluyendo el beneficiario actual)
   if (!ReglasDeBeneficiario.elNumeroCuentaDestinoEsUnico(beneficiariosExistentes, beneficiario.NumeroCuentaDestino, beneficiario.IdCuenta, idBeneficiario))
  {
          return false;
            }

            return await gestionBeneficiarioDA.actualizarBeneficiario(beneficiario, idBeneficiario);
        }

public Task<bool> eliminarBeneficiario(int idBeneficiario)
     {
   return ReglasDeBeneficiario.elIdEsValido(idBeneficiario) ?
     gestionBeneficiarioDA.eliminarBeneficiario(idBeneficiario) :
 Task.FromResult(false);
        }
    }
}
