using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionPagoServicioDA : IGestionPagoServicioDA
    {
        private readonly BancaEnLineaContext bancaEnLineaContext;

        public GestionPagoServicioDA(BancaEnLineaContext bancaEnLineaContext)
 {
    this.bancaEnLineaContext = bancaEnLineaContext;
        }

        public async Task<int> registrarPago(PagoServicio pago)
  {
   try
            {
        var pagoEntidad = new PagoServicioDA
     {
        IdContratoServicio = pago.IdContratoServicio,
          IdCuentaBancariaOrigen = pago.IdCuentaBancariaOrigen,
           Monto = pago.Monto,
             Comision = pago.Comision,
              MontoTotal = pago.MontoTotal,
        FechaCreacion = pago.FechaCreacion,
          FechaEjecucion = pago.FechaEjecucion,
     Estado = pago.Estado
};

  bancaEnLineaContext.PagoServicio.Add(pagoEntidad);
 await bancaEnLineaContext.SaveChangesAsync();
    return pagoEntidad.IdPago;
     }
   catch
   {
                return 0;
}
        }

        public async Task<List<PagoServicio>> obtenerPagosPorCliente(int idCliente)
        {
      var pagos = await bancaEnLineaContext.PagoServicio
    .Include(p => p.ContratoServicio)
      .ThenInclude(c => c.Servicio)
        .Include(p => p.CuentaBancariaOrigen)
          .Where(p => p.CuentaBancariaOrigen.IdCuenta == idCliente)
              .AsNoTracking()
            .OrderByDescending(p => p.FechaCreacion)
.ToListAsync();

      return MapearPagos(pagos);
        }

   public async Task<List<PagoServicio>> obtenerPagosProgramados(int idCliente)
        {
       var pagos = await bancaEnLineaContext.PagoServicio
    .Include(p => p.ContratoServicio)
   .ThenInclude(c => c.Servicio)
          .Include(p => p.CuentaBancariaOrigen)
         .Where(p => p.CuentaBancariaOrigen.IdCuenta == idCliente && p.Estado == EstadoTra.Programada)
      .AsNoTracking()
      .OrderBy(p => p.FechaEjecucion)
      .ToListAsync();

       return MapearPagos(pagos);
        }

     public async Task<PagoServicio?> obtenerPagoPorId(int idPago)
        {
  var pago = await bancaEnLineaContext.PagoServicio
     .Include(p => p.ContratoServicio)
        .ThenInclude(c => c.Servicio)
 .Include(p => p.CuentaBancariaOrigen)
      .FirstOrDefaultAsync(p => p.IdPago == idPago);

            return pago != null ? MapearPago(pago) : null;
        }

        public async Task<bool> cancelarPago(int idPago)
        {
    var pago = await bancaEnLineaContext.PagoServicio.FindAsync(idPago);
        if (pago == null)
return false;

     pago.Estado = EstadoTra.Cancelada;
         await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> actualizarEstado(int idPago, int estado)
        {
    var pago = await bancaEnLineaContext.PagoServicio.FindAsync(idPago);
    if (pago == null)
        return false;

pago.Estado = (EstadoTra)estado;
    await bancaEnLineaContext.SaveChangesAsync();
    return true;
}

public async Task<List<PagoServicio>> obtenerTodosPagos()
{
    var pagos = await bancaEnLineaContext.PagoServicio
        .Include(p => p.ContratoServicio)
     .ThenInclude(c => c.Servicio)
        .Include(p => p.CuentaBancariaOrigen)
      .ThenInclude(cb => cb.Cuenta)
        .AsNoTracking()
   .OrderByDescending(p => p.FechaCreacion)
        .ToListAsync();

    return MapearPagos(pagos);
}

private List<PagoServicio> MapearPagos(List<PagoServicioDA> pagosDA)
{
            return pagosDA.Select(p => MapearPago(p)).ToList();
        }

        private PagoServicio MapearPago(PagoServicioDA p)
        {
            return new PagoServicio
            {
   IdPago = p.IdPago,
   IdContratoServicio = p.IdContratoServicio,
      IdCuentaBancariaOrigen = p.IdCuentaBancariaOrigen,
       Monto = p.Monto,
Comision = p.Comision,
            MontoTotal = p.MontoTotal,
 FechaCreacion = p.FechaCreacion,
   FechaEjecucion = p.FechaEjecucion,
         Estado = p.Estado,
         ContratoServicio = p.ContratoServicio != null ? new ContratoServicio
        {
        IdContratoServicio = p.ContratoServicio.IdContratoServicio,
        IdServicio = p.ContratoServicio.IdServicio,
   NumeroContrato = p.ContratoServicio.NumeroContrato,
          Servicio = p.ContratoServicio.Servicio != null ? new Servicio
   {
     IdServicio = p.ContratoServicio.Servicio.IdServicio,
     Nombre = p.ContratoServicio.Servicio.Nombre,
      Descripcion = p.ContratoServicio.Servicio.Descripcion,
 Contrato = p.ContratoServicio.Servicio.Contrato,
    Costo = p.ContratoServicio.Servicio.Costo
 } : null
          } : null,
       CuentaBancariaOrigen = p.CuentaBancariaOrigen != null ? new CuentaBancaria
     {
   Id = p.CuentaBancariaOrigen.Id,
         NumeroTarjeta = p.CuentaBancariaOrigen.NumeroTarjeta,
     Saldo = p.CuentaBancariaOrigen.Saldo,
       IdCuenta = p.CuentaBancariaOrigen.IdCuenta,
  Cuenta = p.CuentaBancariaOrigen.Cuenta != null ? new Cuenta
      {
      Id = p.CuentaBancariaOrigen.Cuenta.Id,
   Telefono = p.CuentaBancariaOrigen.Cuenta.Telefono,
        Nombre = p.CuentaBancariaOrigen.Cuenta.Nombre,
    PrimerApellido = p.CuentaBancariaOrigen.Cuenta.PrimerApellido,
        SegundoApellido = p.CuentaBancariaOrigen.Cuenta.SegundoApellido,
   Correo = p.CuentaBancariaOrigen.Cuenta.Correo,
 Rol = p.CuentaBancariaOrigen.Cuenta.Rol
     } : null
 } : null
};
  }
  }
}
