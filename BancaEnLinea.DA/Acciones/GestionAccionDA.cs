using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionAccionDA : IGestionAccionDA
    {
        private readonly BancaEnLineaContext bancaEnLineaContext;

   public GestionAccionDA(BancaEnLineaContext bancaEnLineaContext)
        {
    this.bancaEnLineaContext = bancaEnLineaContext;
 }

        public async Task<bool> registrarAccion(Accion accion)
  {
   try
 {
    var accionDA = new AccionDA
    {
         Fecha = DateTime.Now,
              Descripcion = accion.Descripcion,
     IdUsuario = accion.IdUsuario,
   NombreUsuario = accion.NombreUsuario
   };

      bancaEnLineaContext.Accion.Add(accionDA);
    await bancaEnLineaContext.SaveChangesAsync();

     Console.WriteLine($"?? Acción registrada: {accion.Descripcion}");
            return true;
     }
 catch (Exception ex)
       {
        Console.WriteLine($"? Error al registrar acción: {ex.Message}");
         return false;
   }
        }

public async Task<List<Accion>> obtenerTodasLasAcciones()
{
      var acciones = await bancaEnLineaContext.Accion
    .OrderByDescending(a => a.Fecha)
       .AsNoTracking()
        .ToListAsync();

    return acciones.Select(a => new Accion
      {
        Id = a.Id,
        Fecha = a.Fecha,
     Descripcion = a.Descripcion,
    IdUsuario = a.IdUsuario,
       NombreUsuario = a.NombreUsuario
   }).ToList();
        }
    }
}
