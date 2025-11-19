using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferenciasController : ControllerBase
    {
      private readonly IGestionTransferenciaBW gestionTransferenciaBW;

        public TransferenciasController(IGestionTransferenciaBW gestionTransferenciaBW)
        {
 this.gestionTransferenciaBW = gestionTransferenciaBW;
        }

        [HttpPost("RegistrarTransferencia")]
    public async Task<ActionResult> RegistrarTransferencia([FromBody] Transferencia transferencia)
     {
      try
{
                var (exito, mensaje, referencia) = await gestionTransferenciaBW.registrarTransferencia(transferencia);
  
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
 public async Task<ActionResult> AprobarTransferencia(int referencia, [FromQuery] int idAprobador)
        {
    try
       {
     var (exito, mensaje) = await gestionTransferenciaBW.aprobarTransferencia(referencia, idAprobador);
   
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
   public async Task<ActionResult> RechazarTransferencia(int referencia, [FromQuery] int idAprobador, [FromBody] string motivo)
{
  try
     {
var (exito, mensaje) = await gestionTransferenciaBW.rechazarTransferencia(referencia, idAprobador, motivo);
  
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
