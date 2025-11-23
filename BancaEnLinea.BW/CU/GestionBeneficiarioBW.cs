using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BC.Enums;
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
   Console.WriteLine("? ERROR: Beneficiario inválido - datos incompletos o incorrectos");
      return false;
    }

      // Verificar que el cliente (cuenta) exista
            var cliente = await gestionCuentaDA.obtenerCuentaPorId(beneficiario.IdCuenta);
  if (cliente == null)
    {
   Console.WriteLine($"? ERROR: Cliente con ID {beneficiario.IdCuenta} no encontrado");
         return false;
   }

   // Obtener beneficiarios existentes del cliente
     var beneficiariosExistentes = await gestionBeneficiarioDA.obtenerBeneficiariosPorCliente(beneficiario.IdCuenta);
     
     Console.WriteLine($"?? Cliente {beneficiario.IdCuenta} tiene {beneficiariosExistentes.Count} beneficiarios en total");
     Console.WriteLine($"   - Activos: {beneficiariosExistentes.Count(b => b.Estado == EstadoP.Activo)}");
     Console.WriteLine($"   - Pendientes: {beneficiariosExistentes.Count(b => b.Estado == EstadoP.Pendiente)}");
     Console.WriteLine($"   - Inactivos: {beneficiariosExistentes.Count(b => b.Estado == EstadoP.Inactivo)}");

// Validar que no exceda el límite de 3 beneficiarios por cliente
  if (!ReglasDeBeneficiario.puedeAgregarBeneficiario(beneficiariosExistentes, beneficiario.IdCuenta))
      {
          Console.WriteLine($"? ERROR: Límite de 3 beneficiarios alcanzado (Activos + Pendientes)");
     return false;
      }

      // Validar que el alias sea único para este cliente
    if (!ReglasDeBeneficiario.elAliasEsUnico(beneficiariosExistentes, beneficiario.Alias, beneficiario.IdCuenta))
       {
  Console.WriteLine($"? ERROR: El alias '{beneficiario.Alias}' ya existe para este cliente");
    return false;
 }

 // Validar que el número de cuenta destino sea único para este cliente
    if (!ReglasDeBeneficiario.elNumeroCuentaDestinoEsUnico(beneficiariosExistentes, beneficiario.NumeroCuentaDestino, beneficiario.IdCuenta))
   {
          Console.WriteLine($"? ERROR: El número de cuenta {beneficiario.NumeroCuentaDestino} ya existe para este cliente");
 return false;
       }

    Console.WriteLine($"? Todas las validaciones pasaron. Registrando beneficiario '{beneficiario.Alias}'...");
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

     public Task<List<Beneficiarios>> obtenerBeneficiariosPendientes()
        {
       return gestionBeneficiarioDA.obtenerBeneficiariosPendientes();
 }

  public async Task<(bool exito, string mensaje)> confirmarBeneficiario(int idBeneficiario)
        {
    bool resultado = await gestionBeneficiarioDA.confirmarBeneficiario(idBeneficiario);
    return resultado
      ? (true, "Beneficiario confirmado y activado exitosamente")
        : (false, "No se pudo confirmar el beneficiario. Verifique que esté pendiente.");
 }

     public async Task<(bool exito, string mensaje)> rechazarBeneficiario(int idBeneficiario)
     {
    bool resultado = await gestionBeneficiarioDA.rechazarBeneficiario(idBeneficiario);
    return resultado
        ? (true, "Beneficiario rechazado exitosamente")
  : (false, "No se pudo rechazar el beneficiario. Verifique que esté pendiente.");
     }
    }
}
