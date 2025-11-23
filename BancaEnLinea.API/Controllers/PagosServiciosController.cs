using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
  [ApiController]
    public class PagosServiciosController : ControllerBase
    {
   private readonly IGestionPagoServicioBW gestionPagoServicioBW;

public PagosServiciosController(IGestionPagoServicioBW gestionPagoServicioBW)
   {
   this.gestionPagoServicioBW = gestionPagoServicioBW;
}

        [HttpPost("RealizarPago")]
        public async Task<ActionResult> RealizarPago([FromBody] PagoServicioRequest request)
     {
  try
     {
       var (exito, mensaje, idPago) = await gestionPagoServicioBW.realizarPago(request);
            if (exito)
      return Ok(new { success = true, message = mensaje, idPago });

     return BadRequest(new { success = false, message = mensaje });
   }
  catch (Exception ex)
       {
   return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
        }
   }

   [HttpGet("ObtenerPagosPorCliente/{idCliente}")]
     public async Task<ActionResult> ObtenerPagosPorCliente(int idCliente)
        {
 try
   {
      var resultado = await gestionPagoServicioBW.obtenerPagosPorCliente(idCliente);
  return Ok(new { success = true, data = resultado });
   }
  catch (Exception ex)
  {
return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
       }
  }

   [HttpGet("ObtenerPagosProgramados/{idCliente}")]
  public async Task<ActionResult> ObtenerPagosProgramados(int idCliente)
        {
      try
  {
  var resultado = await gestionPagoServicioBW.obtenerPagosProgramados(idCliente);
     return Ok(new { success = true, data = resultado });
   }
 catch (Exception ex)
{
     return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
 }
    }

[HttpPut("CancelarPago/{idPago}")]
  public async Task<ActionResult> CancelarPago(int idPago, [FromQuery] int idCliente)
     {
    try
   {
   var (exito, mensaje) = await gestionPagoServicioBW.cancelarPago(idPago, idCliente);
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
