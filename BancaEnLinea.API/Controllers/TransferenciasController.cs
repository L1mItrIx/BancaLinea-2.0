using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferenciasController : ControllerBase
    {
  private readonly IGestionTransferenciaBW gestionTransferenciaBW;
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;

        public TransferenciasController(
       IGestionTransferenciaBW gestionTransferenciaBW,
    IGestionCuentaBancariaDA gestionCuentaBancariaDA)
      {
            this.gestionTransferenciaBW = gestionTransferenciaBW;
            this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
   }

        [HttpPost("RegistrarTransferencia")]
    public async Task<ActionResult> RegistrarTransferencia([FromBody] TransferenciaRequest request)
     {
      try
{
                var (exito, mensaje, referencia) = await gestionTransferenciaBW.registrarTransferencia(request);
  
 if (exito)
   return Ok(new { success = true, message = mensaje, referencia });

  return BadRequest(new { success = false, message = mensaje });
     }
   catch (Exception ex)
     {
      return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
     }
     }

  [HttpGet("ObtenerTransferenciasPorCuenta/{idCuentaBancaria}")]
   public async Task<ActionResult> ObtenerTransferenciasPorCuenta(int idCuentaBancaria)
{
  try
      {
    var resultado = await gestionTransferenciaBW.obtenerTransferenciasPorCuentaBancaria(idCuentaBancaria);
   return Ok(new { success = true, data = resultado });
      }
          catch (Exception ex)
            {
      return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
       }
        }

[HttpGet("ObtenerTodasLasTransferencias")]
        public async Task<ActionResult> ObtenerTodasLasTransferencias()
 {
     try
   {
var resultado = await gestionTransferenciaBW.obtenerTodasLasTransferencias();
    return Ok(new { success = true, data = resultado });
   }
        catch (Exception ex)
   {
      return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
      }
  }

        [HttpGet("ObtenerTransferenciaPorReferencia/{referencia}")]
        public async Task<ActionResult> ObtenerTransferenciaPorReferencia(int referencia)
  {
   try
   {
    var resultado = await gestionTransferenciaBW.obtenerTransferenciaPorReferencia(referencia);
 
  if (resultado == null)
   return NotFound(new { success = false, message = "Transferencia no encontrada" });

 return Ok(new { success = true, data = resultado });
      }
  catch (Exception ex)
 {
          return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
      }
}

   [HttpGet("ObtenerTransferenciasPendientes")]
        public async Task<ActionResult> ObtenerTransferenciasPendientes()
    {
 try
     {
    var resultado = await gestionTransferenciaBW.obtenerTransferenciasPendientes();
     return Ok(new { success = true, data = resultado });
      }
   catch (Exception ex)
{
     return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
     }
 }

        /// <summary>
        /// Obtiene transferencias enviadas por un cliente con información de monedas
   /// </summary>
        [HttpGet("ObtenerTransferenciasPorCliente/{idCliente}")]
   public async Task<ActionResult> ObtenerTransferenciasPorCliente(int idCliente)
   {
  try
 {
       Console.WriteLine($"?? Obteniendo transferencias enviadas para cliente {idCliente}");
        
     var transferencias = await gestionTransferenciaBW.obtenerTransferenciasPorCliente(idCliente);
    
       var transferenciasConInfo = new List<TransferenciaResponse>();

        foreach (var t in transferencias)
  {
             // Obtener cuenta origen
 var cuentaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(t.IdCuentaBancariaOrigen);
       
          // Obtener cuenta destino (si existe en el sistema)
       var cuentaDestino = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorNumeroTarjeta(t.NumeroCuentaDestino);
   
         // Determinar si hubo conversión
            bool requiereConversion = cuentaDestino != null && cuentaOrigen.Moneda != cuentaDestino.Moneda;
          
         // Calcular monto recibido (con conversión si aplica)
       long? montoRecibido = null;
    decimal? tipoCambio = null;
  
             if (cuentaDestino != null)
         {
     montoRecibido = ReglasDeTransferencia.calcularMontoDestino(
              t.Monto,
       cuentaOrigen.Moneda,
        cuentaDestino.Moneda);
         
        if (requiereConversion)
           {
     tipoCambio = cuentaOrigen.Moneda == Moneda.USD 
  ? ReglasDeConversionMoneda.obtenerTipoCambioUsdACrc()
          : ReglasDeConversionMoneda.obtenerTipoCambioCrcAUsd();
       }
         }

          transferenciasConInfo.Add(new TransferenciaResponse
      {
     Referencia = t.Referencia,
              NumeroCuentaDestino = t.NumeroCuentaDestino,
            FechaCreacion = t.FechaCreacion,
 FechaEjecucion = t.FechaEjecucion,
   Estado = t.Estado,
    EstadoTexto = t.Estado.ToString(),
      Descripcion = t.Descripcion,
       
  NumeroCuentaOrigen = cuentaOrigen.NumeroTarjeta,
        MonedaOrigen = cuentaOrigen.Moneda,
  MonedaOrigenTexto = cuentaOrigen.Moneda == Moneda.CRC ? "CRC" : "USD",
     
          MontoEnviado = t.Monto,
               ComisionEnviada = t.Comision,
        TotalDebitado = t.MontoTotal,
        
         RequiereConversion = requiereConversion,
        MonedaDestino = cuentaDestino?.Moneda,
  MonedaDestinoTexto = cuentaDestino?.Moneda == Moneda.CRC ? "CRC" : cuentaDestino?.Moneda == Moneda.USD ? "USD" : null,
    MontoRecibido = montoRecibido,
   TipoCambioAplicado = tipoCambio
        });
    }

        Console.WriteLine($"? Se encontraron {transferenciasConInfo.Count} transferencias enviadas");
        return Ok(new { success = true, data = transferenciasConInfo, total = transferenciasConInfo.Count });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? ERROR: {ex.Message}");
        return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
    }
}

/// <summary>
/// Obtiene transferencias enviadas por un cliente - ENDPOINT ADICIONAL
/// Mismo que ObtenerTransferenciasPorCliente pero con nombre más descriptivo
/// </summary>
[HttpGet("ObtenerTransferenciasEnviadas/{idCliente}")]
public async Task<ActionResult> ObtenerTransferenciasEnviadas(int idCliente)
{
    // Reutilizar la misma lógica del endpoint anterior
    return await ObtenerTransferenciasPorCliente(idCliente);
}

        /// <summary>
        /// Obtiene transferencias recibidas por un cliente con información de conversión
      /// </summary>
        [HttpGet("ObtenerTransferenciasRecibidas/{idCliente}")]
        public async Task<ActionResult> ObtenerTransferenciasRecibidas(int idCliente)
        {
            try
            {
        Console.WriteLine($"?? Obteniendo transferencias recibidas para cliente {idCliente}");
        
        var transferenciasRecibidas = await gestionTransferenciaBW.obtenerTransferenciasRecibidas(idCliente);
      
        var transferenciasConInfo = new List<object>();

 foreach (var tr in transferenciasRecibidas)
        {
            // Obtener cuenta origen por número de cuenta
   var cuentaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorNumeroTarjeta(tr.CuentaOrigen);
 
            if (cuentaOrigen != null)
 {
      // Obtener todas las cuentas del cliente para encontrar en cuál recibió
           var cuentasCliente = await gestionCuentaBancariaDA.obtenerCuentasBancarias(idCliente);
             
   // Por ahora, asumimos que recibió en la primera cuenta activa que encuentre
           // En el futuro, podrías guardar el IdCuentaDestino en la transferencia
        var tuCuenta = cuentasCliente.FirstOrDefault();
            
     if (tuCuenta != null)
           {
    // Determinar si hubo conversión
        bool huboConversion = cuentaOrigen.Moneda != tuCuenta.Moneda;
      
          // Calcular monto recibido en tu moneda
   long montoRecibido = ReglasDeTransferencia.calcularMontoDestino(
    tr.Monto,
              cuentaOrigen.Moneda,
        tuCuenta.Moneda);
     
      decimal? tipoCambio = null;
         if (huboConversion)
        {
tipoCambio = cuentaOrigen.Moneda == Moneda.USD 
              ? ReglasDeConversionMoneda.obtenerTipoCambioUsdACrc()
          : ReglasDeConversionMoneda.obtenerTipoCambioCrcAUsd();
  }

           transferenciasConInfo.Add(new
    {
     referencia = tr.Referencia,
         numeroCuentaOrigen = tr.CuentaOrigen,
         remitente = tr.Remitente,
       fechaRecepcion = tr.FechaRecepcion,
  descripcion = tr.Descripcion,
   
          monedaOrigen = cuentaOrigen.Moneda,
      monedaOrigenTexto = cuentaOrigen.Moneda == Moneda.CRC ? "CRC" : "USD",
       monedaTuCuenta = tuCuenta.Moneda,
        monedaTuCuentaTexto = tuCuenta.Moneda == Moneda.CRC ? "CRC" : "USD",
           
   montoEnviadoPorRemitente = tr.Monto,
              montoRecibidoEnTuCuenta = montoRecibido,
       
 huboConversion = huboConversion,
             tipoCambioAplicado = tipoCambio,
    
               // Información adicional útil
            simboloMonedaOrigen = cuentaOrigen.Moneda == Moneda.CRC ? "?" : "$",
       simboloMonedaTuCuenta = tuCuenta.Moneda == Moneda.CRC ? "?" : "$"
     });
     }
      }
  }

        Console.WriteLine($"? Se encontraron {transferenciasConInfo.Count} transferencias recibidas");
        return Ok(new { success = true, data = transferenciasConInfo, total = transferenciasConInfo.Count });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? ERROR: {ex.Message}");
   return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
    }
}

  [HttpPut("CancelarTransferencia/{referencia}")]
        public async Task<ActionResult> CancelarTransferencia(int referencia, [FromQuery] int idCliente)
  {
  try
 {
     var (exito, mensaje) = await gestionTransferenciaBW.cancelarTransferencia(referencia, idCliente);
      
   if (exito)
   return Ok(new { success = true, message = mensaje });

       return BadRequest(new { success = false, message = mensaje });
  }
catch (Exception ex)
  {
  return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
}
   }

 [HttpPut("AprobarTransferencia/{referencia}")]
 public async Task<ActionResult> AprobarTransferencia(int referencia)
        {
    try
     {
     var (exito, mensaje) = await gestionTransferenciaBW.aprobarTransferencia(referencia);
   
      if (exito)
     return Ok(new { success = true, message = mensaje });

       return BadRequest(new { success = false, message = mensaje });
 }
 catch (Exception ex)
      {
        return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
     }
 }

  [HttpPut("RechazarTransferencia/{referencia}")]
   public async Task<ActionResult> RechazarTransferencia(int referencia, [FromBody] string motivo)
{
  try
     {
var (exito, mensaje) = await gestionTransferenciaBW.rechazarTransferencia(referencia, motivo);
  
if (exito)
     return Ok(new { success = true, message = mensaje });

    return BadRequest(new { success = false, message = mensaje });
    }
catch (Exception ex)
{
 return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
     }
 }
    }
}
