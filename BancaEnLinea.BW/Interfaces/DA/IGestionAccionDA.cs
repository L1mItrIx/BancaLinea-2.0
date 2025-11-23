using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.DA
{
    public interface IGestionAccionDA
    {
        Task<bool> registrarAccion(Accion accion);
   Task<List<Accion>> obtenerTodasLasAcciones();
    }
}
