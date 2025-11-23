using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionServicioDA : IGestionServicioDA
    {
        private readonly BancaEnLineaContext bancaEnLineaContext;

        public GestionServicioDA(BancaEnLineaContext bancaEnLineaContext)
        {
            this.bancaEnLineaContext = bancaEnLineaContext;
        }

        public async Task<bool> registrarServicio(Servicio servicio)
        {
  try
            {
    var servicioEntidad = new ServicioDA
   {
       Nombre = servicio.Nombre,
           Descripcion = servicio.Descripcion,
       Contrato = servicio.Contrato,
         Costo = servicio.Costo
            };

 bancaEnLineaContext.Servicio.Add(servicioEntidad);
await bancaEnLineaContext.SaveChangesAsync();
 return true;
         }
            catch
   {
         return false;
            }
    }

        public async Task<List<Servicio>> obtenerTodosLosServicios()
{
            var servicios = await bancaEnLineaContext.Servicio
     .AsNoTracking()
   .ToListAsync();

            return servicios.Select(s => new Servicio
{
          IdServicio = s.IdServicio,
        Nombre = s.Nombre,
       Descripcion = s.Descripcion,
    Contrato = s.Contrato,
        Costo = s.Costo
            }).ToList();
        }

        public async Task<Servicio?> obtenerServicioPorId(int idServicio)
      {
            var servicio = await bancaEnLineaContext.Servicio
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdServicio == idServicio);

            if (servicio == null)
  return null;

        return new Servicio
      {
       IdServicio = servicio.IdServicio,
                Nombre = servicio.Nombre,
  Descripcion = servicio.Descripcion,
    Contrato = servicio.Contrato,
                Costo = servicio.Costo
     };
  }

        public async Task<bool> actualizarServicio(Servicio servicio, int idServicio)
        {
            var servicioExistente = await bancaEnLineaContext.Servicio.FindAsync(idServicio);
     if (servicioExistente == null)
        return false;

            servicioExistente.Nombre = servicio.Nombre;
       servicioExistente.Descripcion = servicio.Descripcion;
   servicioExistente.Contrato = servicio.Contrato;
      servicioExistente.Costo = servicio.Costo;

            await bancaEnLineaContext.SaveChangesAsync();
    return true;
        }

    public async Task<bool> eliminarServicio(int idServicio)
        {
            var servicio = await bancaEnLineaContext.Servicio.FindAsync(idServicio);
         if (servicio == null)
     return false;

     bancaEnLineaContext.Servicio.Remove(servicio);
            await bancaEnLineaContext.SaveChangesAsync();
return true;
        }
    }
}
