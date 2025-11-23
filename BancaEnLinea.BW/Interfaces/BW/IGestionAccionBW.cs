using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionAccionBW
    {
        Task<bool> registrarAccion(Accion accion);
        Task<List<Accion>> obtenerTodasLasAcciones();
    }
}
