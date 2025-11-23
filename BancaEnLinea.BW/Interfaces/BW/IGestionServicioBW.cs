using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionServicioBW
    {
        Task<bool> registrarServicio(Servicio servicio);
        Task<List<Servicio>> obtenerTodosLosServicios();
     Task<Servicio?> obtenerServicioPorId(int idServicio);
        Task<bool> actualizarServicio(Servicio servicio, int idServicio);
    Task<bool> eliminarServicio(int idServicio);
    }
}
