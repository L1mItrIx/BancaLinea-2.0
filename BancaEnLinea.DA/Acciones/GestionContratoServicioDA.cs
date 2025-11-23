using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionContratoServicioDA : IGestionContratoServicioDA
  {
        private readonly BancaEnLineaContext bancaEnLineaContext;

        public GestionContratoServicioDA(BancaEnLineaContext bancaEnLineaContext)
 {
            this.bancaEnLineaContext = bancaEnLineaContext;
        }

      public async Task<bool> agregarContrato(ContratoServicio contrato)
{
      try
            {
                var contratoEntidad = new ContratoServicioDA
     {
           IdServicio = contrato.IdServicio,
            NumeroContrato = contrato.NumeroContrato
            };

     bancaEnLineaContext.ContratoServicio.Add(contratoEntidad);
                await bancaEnLineaContext.SaveChangesAsync();
             return true;
    }
    catch
            {
       return false;
    }
        }

        public async Task<List<ContratoServicio>> obtenerTodosLosContratos()
{
        var contratos = await bancaEnLineaContext.ContratoServicio
                .Include(c => c.Servicio)
   .AsNoTracking()
 .ToListAsync();

            return contratos.Select(c => new ContratoServicio
     {
         IdContratoServicio = c.IdContratoServicio,
         IdServicio = c.IdServicio,
     NumeroContrato = c.NumeroContrato,
           Servicio = c.Servicio != null ? new Servicio
{
            IdServicio = c.Servicio.IdServicio,
             Nombre = c.Servicio.Nombre,
         Descripcion = c.Servicio.Descripcion,
        Contrato = c.Servicio.Contrato,
      Costo = c.Servicio.Costo
        } : null
         }).ToList();
 }

public async Task<List<ContratoServicio>> obtenerContratosPorServicio(int idServicio)
        {
          var contratos = await bancaEnLineaContext.ContratoServicio
           .Where(c => c.IdServicio == idServicio)
         .Include(c => c.Servicio)
    .AsNoTracking()
                .ToListAsync();

   return contratos.Select(c => new ContratoServicio
      {
             IdContratoServicio = c.IdContratoServicio,
      IdServicio = c.IdServicio,
           NumeroContrato = c.NumeroContrato,
  Servicio = c.Servicio != null ? new Servicio
   {
   IdServicio = c.Servicio.IdServicio,
         Nombre = c.Servicio.Nombre,
    Descripcion = c.Servicio.Descripcion,
        Contrato = c.Servicio.Contrato,
              Costo = c.Servicio.Costo
            } : null
            }).ToList();
        }

        public async Task<ContratoServicio?> obtenerContratoPorNumero(long numeroContrato)
        {
  var contrato = await bancaEnLineaContext.ContratoServicio
    .Include(c => c.Servicio)
   .FirstOrDefaultAsync(c => c.NumeroContrato == numeroContrato);

    if (contrato == null)
             return null;

 return new ContratoServicio
{
    IdContratoServicio = contrato.IdContratoServicio,
  IdServicio = contrato.IdServicio,
     NumeroContrato = contrato.NumeroContrato,
  Servicio = contrato.Servicio != null ? new Servicio
        {
      IdServicio = contrato.Servicio.IdServicio,
              Nombre = contrato.Servicio.Nombre,
         Descripcion = contrato.Servicio.Descripcion,
          Contrato = contrato.Servicio.Contrato,
           Costo = contrato.Servicio.Costo
       } : null
        };
        }

        public async Task<bool> eliminarContrato(int idContratoServicio)
        {
  var contrato = await bancaEnLineaContext.ContratoServicio.FindAsync(idContratoServicio);
            if (contrato == null)
    return false;

      bancaEnLineaContext.ContratoServicio.Remove(contrato);
 await bancaEnLineaContext.SaveChangesAsync();
    return true;
 }

      public async Task<List<ContratoServicio>> obtenerTodosLosContratosPendientes()
   {
     // Obtener IDs de contratos que tienen pagos EXITOSOS (Estado = 2)
        var idsContratosPagados = await bancaEnLineaContext.PagoServicio
.Where(p => p.Estado == EstadoTra.Exitosa)
       .Select(p => p.IdContratoServicio)
    .Distinct()
      .ToListAsync();

   // Obtener TODOS los contratos que NO están en la lista de pagados
       var contratos = await bancaEnLineaContext.ContratoServicio
   .Include(c => c.Servicio)
           .Where(c => !idsContratosPagados.Contains(c.IdContratoServicio))
           .AsNoTracking()
      .ToListAsync();

            return contratos.Select(c => new ContratoServicio
    {
     IdContratoServicio = c.IdContratoServicio,
   IdServicio = c.IdServicio,
         NumeroContrato = c.NumeroContrato,
       Servicio = c.Servicio != null ? new Servicio
        {
    IdServicio = c.Servicio.IdServicio,
  Nombre = c.Servicio.Nombre,
 Descripcion = c.Servicio.Descripcion,
       Contrato = c.Servicio.Contrato,
          Costo = c.Servicio.Costo
 } : null
     }).ToList();
        }
 }
}
