using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
    public class GestionAccionBW : IGestionAccionBW
    {
 private readonly IGestionAccionDA gestionAccionDA;

        public GestionAccionBW(IGestionAccionDA gestionAccionDA)
   {
       this.gestionAccionDA = gestionAccionDA;
        }

     public Task<bool> registrarAccion(Accion accion)
  {
       return gestionAccionDA.registrarAccion(accion);
        }

        public Task<List<Accion>> obtenerTodasLasAcciones()
   {
   return gestionAccionDA.obtenerTodasLasAcciones();
 }
    }
}
