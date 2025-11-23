using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionPagoServicioBW
    {
     Task<(bool exito, string mensaje, int? idPago)> realizarPago(PagoServicioRequest request);
     Task<List<PagoServicio>> obtenerPagosPorCliente(int idCliente);
 Task<List<PagoServicio>> obtenerPagosProgramados(int idCliente);
        Task<(bool exito, string mensaje)> cancelarPago(int idPago, int idCliente);
        Task<List<PagoServicio>> obtenerTodosPagos();
    }
}
