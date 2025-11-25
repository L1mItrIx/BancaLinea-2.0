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
            if (!ReglasDeBeneficiario.elBeneficiarioEsValido(beneficiario))
            {
      Console.WriteLine("? ERROR: Beneficiario inválido - datos incompletos o incorrectos");
         return false;
   }

   var cliente = await gestionCuentaDA.obtenerCuentaPorId(beneficiario.IdCuenta);
         if (cliente == null)
            {
  Console.WriteLine($"? ERROR: Cliente con ID {beneficiario.IdCuenta} no encontrado");
return false;
         }

      var beneficiariosExistentes = await gestionBeneficiarioDA.obtenerBeneficiariosPorCliente(beneficiario.IdCuenta);
mostrarEstadisticasBeneficiarios(beneficiario.IdCuenta, beneficiariosExistentes);

    var validacion = validarReglasDeNegocio(beneficiario, beneficiariosExistentes);
        if (!validacion.esValido)
     {
            Console.WriteLine($"? ERROR: {validacion.mensaje}");
       return false;
   }

 Console.WriteLine($"? Todas las validaciones pasaron. Registrando beneficiario '{beneficiario.Alias}'...");
      return await gestionBeneficiarioDA.registrarBeneficiario(beneficiario);
      }

private void mostrarEstadisticasBeneficiarios(int idCuenta, List<Beneficiarios> beneficiariosExistentes)
        {
      Console.WriteLine($"?? Cliente {idCuenta} tiene {beneficiariosExistentes.Count} beneficiarios en total");
       Console.WriteLine($"   - Activos: {beneficiariosExistentes.Count(b => b.Estado == EstadoP.Activo)}");
    Console.WriteLine($"   - Pendientes: {beneficiariosExistentes.Count(b => b.Estado == EstadoP.Pendiente)}");
   Console.WriteLine($"   - Inactivos: {beneficiariosExistentes.Count(b => b.Estado == EstadoP.Inactivo)}");
   }

        private (bool esValido, string mensaje) validarReglasDeNegocio(Beneficiarios beneficiario, List<Beneficiarios> beneficiariosExistentes)
   {
        if (!ReglasDeBeneficiario.puedeAgregarBeneficiario(beneficiariosExistentes, beneficiario.IdCuenta))
         return (false, "Límite de 3 beneficiarios alcanzado (Activos + Pendientes)");

   if (!ReglasDeBeneficiario.elAliasEsUnico(beneficiariosExistentes, beneficiario.Alias, beneficiario.IdCuenta))
          return (false, $"El alias '{beneficiario.Alias}' ya existe para este cliente");

       if (!ReglasDeBeneficiario.elNumeroCuentaDestinoEsUnico(beneficiariosExistentes, beneficiario.NumeroCuentaDestino, beneficiario.IdCuenta))
           return (false, $"El número de cuenta {beneficiario.NumeroCuentaDestino} ya existe para este cliente");

    return (true, string.Empty);
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
        return false;

            var beneficiarioExistente = await gestionBeneficiarioDA.obtenerBeneficiarioPorId(idBeneficiario);
        if (beneficiarioExistente == null)
       return false;

   beneficiario.IdCuenta = beneficiarioExistente.IdCuenta;

        if (!ReglasDeBeneficiario.elBeneficiarioEsValido(beneficiario))
    return false;

       var beneficiariosExistentes = await gestionBeneficiarioDA.obtenerBeneficiariosPorCliente(beneficiario.IdCuenta);

            var validacion = validarUnicidadAlActualizar(beneficiario, beneficiariosExistentes, idBeneficiario);
if (!validacion.esValido)
          return false;

       return await gestionBeneficiarioDA.actualizarBeneficiario(beneficiario, idBeneficiario);
        }

        private (bool esValido, string mensaje) validarUnicidadAlActualizar(
 Beneficiarios beneficiario, List<Beneficiarios> beneficiariosExistentes, int idBeneficiario)
        {
if (!ReglasDeBeneficiario.elAliasEsUnico(beneficiariosExistentes, beneficiario.Alias, beneficiario.IdCuenta, idBeneficiario))
       return (false, "El alias ya existe");

if (!ReglasDeBeneficiario.elNumeroCuentaDestinoEsUnico(beneficiariosExistentes, beneficiario.NumeroCuentaDestino, beneficiario.IdCuenta, idBeneficiario))
         return (false, "El número de cuenta ya existe");

            return (true, string.Empty);
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
