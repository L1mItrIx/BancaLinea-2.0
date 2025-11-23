using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.DA
{
    public interface IGestionServicioDA
    {
        Task<bool> registrarServicio(Servicio servicio);
        Task<List<Servicio>> obtenerTodosLosServicios();
        Task<Servicio?> obtenerServicioPorId(int idServicio);
     Task<bool> actualizarServicio(Servicio servicio, int idServicio);
        Task<bool> eliminarServicio(int idServicio);
    }
}
