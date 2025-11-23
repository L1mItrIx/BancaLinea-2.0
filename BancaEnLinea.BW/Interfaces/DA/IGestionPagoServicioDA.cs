using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.DA
{
    public interface IGestionPagoServicioDA
    {
        Task<int> registrarPago(PagoServicio pago);
        Task<List<PagoServicio>> obtenerPagosPorCliente(int idCliente);
        Task<List<PagoServicio>> obtenerPagosProgramados(int idCliente);
        Task<PagoServicio?> obtenerPagoPorId(int idPago);
        Task<bool> cancelarPago(int idPago);
        Task<bool> actualizarEstado(int idPago, int nuevoEstado);
        Task<List<PagoServicio>> obtenerTodosPagos();
    }
}
