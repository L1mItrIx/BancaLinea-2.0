using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
    public class GestionServicioBW : IGestionServicioBW
{
  private readonly IGestionServicioDA gestionServicioDA;

   public GestionServicioBW(IGestionServicioDA gestionServicioDA)
    {
   this.gestionServicioDA = gestionServicioDA;
        }

 public Task<bool> registrarServicio(Servicio servicio)
{
   return gestionServicioDA.registrarServicio(servicio);
     }

        public Task<List<Servicio>> obtenerTodosLosServicios()
        {
    return gestionServicioDA.obtenerTodosLosServicios();
      }

        public Task<Servicio?> obtenerServicioPorId(int idServicio)
     {
       return gestionServicioDA.obtenerServicioPorId(idServicio);
 }

        public Task<bool> actualizarServicio(Servicio servicio, int idServicio)
        {
      return gestionServicioDA.actualizarServicio(servicio, idServicio);
      }

        public Task<bool> eliminarServicio(int idServicio)
        {
 return gestionServicioDA.eliminarServicio(idServicio);
        }
    }
}
