using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly IGestionTransferenciaBW gestionTransferenciaBW;
        private readonly IGestionPagoServicioBW gestionPagoServicioBW;
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;
        private readonly IGestionCuentaDA gestionCuentaDA;

     public HistorialController(
        IGestionTransferenciaBW gestionTransferenciaBW,
IGestionPagoServicioBW gestionPagoServicioBW,
            IGestionCuentaBancariaDA gestionCuentaBancariaDA,
         IGestionCuentaDA gestionCuentaDA)
        {
this.gestionTransferenciaBW = gestionTransferenciaBW;
            this.gestionPagoServicioBW = gestionPagoServicioBW;
 this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
      this.gestionCuentaDA = gestionCuentaDA;
        }

    [HttpGet("ObtenerHistorialTransacciones")]
   public async Task<ActionResult> ObtenerHistorialTransacciones()
    {
        try
            {
       Console.WriteLine("?? Obteniendo historial completo de transacciones...");

var historial = new List<TransaccionHistorial>();
var transferencias = await gestionTransferenciaBW.obtenerTodasLasTransferencias();

 foreach (var t in transferencias)
           {
       var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(t.IdCuentaBancariaOrigen);
        var cliente = await gestionCuentaDA.obtenerCuentaPorId(cuentaBancaria.IdCuenta);

            historial.Add(new TransaccionHistorial
{
          Id = t.Referencia,
            FechaCreacion = t.FechaCreacion,
           FechaEjecucion = t.FechaEjecucion,
  Tipo = "Transferencia",
      TipoIcono = "transfer",
     IdCliente = cliente.Id,
            NombreCliente = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}",
          NumeroCuenta = cuentaBancaria.NumeroTarjeta,
          MonedaCuenta = cuentaBancaria.Moneda,
   MonedaCuentaTexto = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD",
                SimboloMoneda = cuentaBancaria.Moneda == Moneda.CRC ? "?" : "$",
  Monto = t.Monto,
     Comision = t.Comision,
         MontoTotal = t.MontoTotal,
      Estado = t.Estado,
         EstadoTexto = t.Estado.ToString(),
        Descripcion = t.Descripcion ?? "Transferencia bancaria",
      Destino = t.NumeroCuentaDestino.ToString(),
          Referencia = $"TRF-{t.Referencia}"
       });
           }

     var pagos = await gestionPagoServicioBW.obtenerTodosPagos();

      foreach (var p in pagos)
                {
       var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(p.IdCuentaBancariaOrigen);
  var cliente = await gestionCuentaDA.obtenerCuentaPorId(cuentaBancaria.IdCuenta);

                historial.Add(new TransaccionHistorial
            {
       Id = p.IdPago,
           FechaCreacion = p.FechaCreacion,
    FechaEjecucion = p.FechaEjecucion,
         Tipo = "Pago de Servicio",
     TipoIcono = "payment",
      IdCliente = cliente.Id,
          NombreCliente = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}",
   NumeroCuenta = cuentaBancaria.NumeroTarjeta,
             MonedaCuenta = cuentaBancaria.Moneda,
   MonedaCuentaTexto = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD",
            SimboloMoneda = cuentaBancaria.Moneda == Moneda.CRC ? "?" : "$",
                  Monto = p.Monto,
  Comision = p.Comision,
    MontoTotal = p.MontoTotal,
         Estado = p.Estado,
           EstadoTexto = p.Estado.ToString(),
               Descripcion = p.ContratoServicio?.Servicio?.Nombre ?? "Pago de servicio",
   Destino = p.ContratoServicio?.NumeroContrato.ToString() ?? "N/A",
   Referencia = $"PAG-{p.IdPago}"
           });
          }

     var historialOrdenado = historial.OrderByDescending(h => h.FechaCreacion).ToList();
           Console.WriteLine($"? Se encontraron {historialOrdenado.Count} transacciones");

                return Ok(new
         {
         success = true,
        data = historialOrdenado,
        total = historialOrdenado.Count
         });
      }
            catch (Exception ex)
            {
        Console.WriteLine($"? ERROR: {ex.Message}");
return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
      }
        }

        [HttpGet("ObtenerHistorialPorCliente/{idCliente}")]
        public async Task<ActionResult> ObtenerHistorialPorCliente(int idCliente)
        {
     try
  {
       var historial = new List<TransaccionHistorial>();
        var cliente = await gestionCuentaDA.obtenerCuentaPorId(idCliente);

              if (cliente == null)
                return NotFound(new { success = false, message = "Cliente no encontrado" });

        var transferencias = await gestionTransferenciaBW.obtenerTransferenciasPorCliente(idCliente);

        foreach (var t in transferencias)
  {
       var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(t.IdCuentaBancariaOrigen);

     historial.Add(new TransaccionHistorial
      {
          Id = t.Referencia,
       FechaCreacion = t.FechaCreacion,
FechaEjecucion = t.FechaEjecucion,
               Tipo = "Transferencia",
            TipoIcono = "transfer",
      IdCliente = cliente.Id,
    NombreCliente = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}",
         NumeroCuenta = cuentaBancaria.NumeroTarjeta,
           MonedaCuenta = cuentaBancaria.Moneda,
      MonedaCuentaTexto = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD",
      SimboloMoneda = cuentaBancaria.Moneda == Moneda.CRC ? "?" : "$",
           Monto = t.Monto,
                Comision = t.Comision,
       MontoTotal = t.MontoTotal,
             Estado = t.Estado,
             EstadoTexto = t.Estado.ToString(),
             Descripcion = t.Descripcion ?? "Transferencia bancaria",
  Destino = t.NumeroCuentaDestino.ToString(),
               Referencia = $"TRF-{t.Referencia}"
             });
       }

    var pagos = await gestionPagoServicioBW.obtenerPagosPorCliente(idCliente);

      foreach (var p in pagos)
  {
                var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(p.IdCuentaBancariaOrigen);

         historial.Add(new TransaccionHistorial
   {
              Id = p.IdPago,
    FechaCreacion = p.FechaCreacion,
              FechaEjecucion = p.FechaEjecucion,
             Tipo = "Pago de Servicio",
          TipoIcono = "payment",
               IdCliente = cliente.Id,
      NombreCliente = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}",
            NumeroCuenta = cuentaBancaria.NumeroTarjeta,
          MonedaCuenta = cuentaBancaria.Moneda,
    MonedaCuentaTexto = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD",
               SimboloMoneda = cuentaBancaria.Moneda == Moneda.CRC ? "?" : "$",
  Monto = p.Monto,
  Comision = p.Comision,
            MontoTotal = p.MontoTotal,
  Estado = p.Estado,
        EstadoTexto = p.Estado.ToString(),
          Descripcion = p.ContratoServicio?.Servicio?.Nombre ?? "Pago de servicio",
      Destino = p.ContratoServicio?.NumeroContrato.ToString() ?? "N/A",
      Referencia = $"PAG-{p.IdPago}"
           });
     }

      var historialOrdenado = historial.OrderByDescending(h => h.FechaCreacion).ToList();
     return Ok(new { success = true, data = historialOrdenado, total = historialOrdenado.Count });
            }
 catch (Exception ex)
            {
     return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }

  [HttpGet("ObtenerHistorialPorCuenta/{idCuentaBancaria}")]
        public async Task<ActionResult> ObtenerHistorialPorCuenta(int idCuentaBancaria)
 {
          try
            {
          var historial = new List<TransaccionHistorial>();
     var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(idCuentaBancaria);

  if (cuentaBancaria == null)
     return NotFound(new { success = false, message = "Cuenta bancaria no encontrada" });

   var cliente = await gestionCuentaDA.obtenerCuentaPorId(cuentaBancaria.IdCuenta);
        var transferencias = await gestionTransferenciaBW.obtenerTransferenciasPorCuentaBancaria(idCuentaBancaria);

    foreach (var t in transferencias)
      {
       historial.Add(new TransaccionHistorial
           {
             Id = t.Referencia,
   FechaCreacion = t.FechaCreacion,
         FechaEjecucion = t.FechaEjecucion,
      Tipo = "Transferencia",
               TipoIcono = "transfer",
 IdCliente = cliente.Id,
         NombreCliente = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}",
            NumeroCuenta = cuentaBancaria.NumeroTarjeta,
     MonedaCuenta = cuentaBancaria.Moneda,
       MonedaCuentaTexto = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD",
     SimboloMoneda = cuentaBancaria.Moneda == Moneda.CRC ? "?" : "$",
               Monto = t.Monto,
         Comision = t.Comision,
             MontoTotal = t.MontoTotal,
 Estado = t.Estado,
 EstadoTexto = t.Estado.ToString(),
             Descripcion = t.Descripcion ?? "Transferencia bancaria",
    Destino = t.NumeroCuentaDestino.ToString(),
         Referencia = $"TRF-{t.Referencia}"
      });
                }

var todosPagos = await gestionPagoServicioBW.obtenerPagosPorCliente(cliente.Id);
         var pagosDeCuenta = todosPagos.Where(p => p.IdCuentaBancariaOrigen == idCuentaBancaria).ToList();

                foreach (var p in pagosDeCuenta)
{
      historial.Add(new TransaccionHistorial
        {
   Id = p.IdPago,
   FechaCreacion = p.FechaCreacion,
      FechaEjecucion = p.FechaEjecucion,
     Tipo = "Pago de Servicio",
   TipoIcono = "payment",
        IdCliente = cliente.Id,
        NombreCliente = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}",
   NumeroCuenta = cuentaBancaria.NumeroTarjeta,
          MonedaCuenta = cuentaBancaria.Moneda,
          MonedaCuentaTexto = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD",
  SimboloMoneda = cuentaBancaria.Moneda == Moneda.CRC ? "?" : "$",
        Monto = p.Monto,
            Comision = p.Comision,
            MontoTotal = p.MontoTotal,
     Estado = p.Estado,
    EstadoTexto = p.Estado.ToString(),
        Descripcion = p.ContratoServicio?.Servicio?.Nombre ?? "Pago de servicio",
             Destino = p.ContratoServicio?.NumeroContrato.ToString() ?? "N/A",
             Referencia = $"PAG-{p.IdPago}"
    });
                }

   var historialOrdenado = historial.OrderByDescending(h => h.FechaCreacion).ToList();
                return Ok(new
   {
           success = true,
           data = historialOrdenado,
total = historialOrdenado.Count,
         numeroCuenta = cuentaBancaria.NumeroTarjeta,
        moneda = cuentaBancaria.Moneda == Moneda.CRC ? "CRC" : "USD"
       });
       }
   catch (Exception ex)
     {
      return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
   }
     }
    }
}
