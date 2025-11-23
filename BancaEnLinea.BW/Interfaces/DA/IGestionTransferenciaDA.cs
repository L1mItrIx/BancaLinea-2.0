using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.DA
{
    public interface IGestionTransferenciaDA
    {
        Task<int> registrarTransferencia(Transferencia transferencia);
        Task<List<Transferencia>> obtenerTransferenciasPorCuentaBancaria(int idCuentaBancaria);
        Task<List<Transferencia>> obtenerTodasLasTransferencias();
        Task<Transferencia?> obtenerTransferenciaPorReferencia(int referencia);
        Task<List<Transferencia>> obtenerTransferenciasPendientes();
        Task<List<Transferencia>> obtenerTransferenciasDelDia(int idCuentaBancaria);
        Task<bool> actualizarEstado(int referencia, int estado, int? idAprobador = null, string? descripcion = null);
        Task<bool> actualizarSaldo(int referencia, long saldoPosterior);
        Task<List<Transferencia>> obtenerTransferenciasPorCliente(int idCliente);
        Task<List<TransferenciaRecibida>> obtenerTransferenciasRecibidas(int idCliente);
    }
}
