using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
  [ApiController]
  public class ContratosServiciosController : ControllerBase
    {
        private readonly IGestionContratoServicioBW gestionContratoServicioBW;

        public ContratosServiciosController(IGestionContratoServicioBW gestionContratoServicioBW)
        {
    this.gestionContratoServicioBW = gestionContratoServicioBW;
        }

   [HttpPost("AgregarContrato")]
        public async Task<ActionResult> AgregarContrato([FromBody] ContratoServicio contrato)
     {
   try
   {
       var (exito, mensaje) = await gestionContratoServicioBW.agregarContrato(contrato);
if (exito)
         return Ok(new { success = true, message = mensaje });

  return BadRequest(new { success = false, message = mensaje });
            }
  catch (Exception ex)
{
     return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
    }
  }

  [HttpGet("ObtenerTodosLosContratos")]
     public async Task<ActionResult> ObtenerTodosLosContratos()
{
       try
   {
      var resultado = await gestionContratoServicioBW.obtenerTodosLosContratos();
     return Ok(new { success = true, data = resultado });
   }
    catch (Exception ex)
  {
            return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
  }
  }

        [HttpGet("ObtenerContratosPorServicio/{idServicio}")]
  public async Task<ActionResult> ObtenerContratosPorServicio(int idServicio)
  {
  try
  {
       var resultado = await gestionContratoServicioBW.obtenerContratosPorServicio(idServicio);
  return Ok(new { success = true, data = resultado });
            }
catch (Exception ex)
{
       return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
   }
   }

  [HttpDelete("EliminarContrato/{idContratoServicio}")]
public async Task<ActionResult> EliminarContrato(int idContratoServicio)
        {
   try
  {
 var (exito, mensaje) = await gestionContratoServicioBW.eliminarContrato(idContratoServicio);
if (exito)
      return Ok(new { success = true, message = mensaje });

return BadRequest(new { success = false, message = mensaje });
  }
      catch (Exception ex)
     {
       return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
   }
    }

 [HttpGet("ObtenerTodosLosContratosPendientes")]
   public async Task<ActionResult> ObtenerTodosLosContratosPendientes()
  {
   try
         {
    var resultado = await gestionContratoServicioBW.obtenerTodosLosContratosPendientes();
   return Ok(new { success = true, data = resultado });
       }
        catch (Exception ex)
   {
     return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
    }
 }
    }
}
