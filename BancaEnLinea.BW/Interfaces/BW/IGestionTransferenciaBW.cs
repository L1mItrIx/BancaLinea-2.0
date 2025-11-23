using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionTransferenciaBW
    {
        Task<(bool exito, string mensaje, int? referencia)> registrarTransferencia(TransferenciaRequest request);
        Task<List<Transferencia>> obtenerTransferenciasPorCuentaBancaria(int idCuentaBancaria);
        Task<List<Transferencia>> obtenerTodasLasTransferencias();
        Task<Transferencia?> obtenerTransferenciaPorReferencia(int referencia);
        Task<List<Transferencia>> obtenerTransferenciasPendientes();
        Task<(bool exito, string mensaje)> cancelarTransferencia(int referencia, int idCliente);
        Task<(bool exito, string mensaje)> aprobarTransferencia(int referencia);
        Task<(bool exito, string mensaje)> rechazarTransferencia(int referencia, string motivo);
        Task<List<Transferencia>> obtenerTransferenciasPorCliente(int idCliente);
        Task<List<TransferenciaRecibida>> obtenerTransferenciasRecibidas(int idCliente);
    }
}
