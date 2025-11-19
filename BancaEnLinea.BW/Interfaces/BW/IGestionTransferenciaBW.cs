using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionTransferenciaBW
  {
      Task<(bool exito, string mensaje, int? referencia)> registrarTransferencia(Transferencia transferencia);
      Task<List<Transferencia>> obtenerTransferenciasPorCuentaBancaria(int idCuentaBancaria);
      Task<List<Transferencia>> obtenerTodasLasTransferencias();
        Task<Transferencia?> obtenerTransferenciaPorReferencia(int referencia);
        Task<List<Transferencia>> obtenerTransferenciasPendientes();
 Task<(bool exito, string mensaje)> cancelarTransferencia(int referencia, int idCliente);
        Task<(bool exito, string mensaje)> aprobarTransferencia(int referencia, int idAprobador);
   Task<(bool exito, string mensaje)> rechazarTransferencia(int referencia, int idAprobador, string motivo);
    }
}
