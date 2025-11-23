using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
  [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly IGestionServicioBW gestionServicioBW;

        public ServiciosController(IGestionServicioBW gestionServicioBW)
        {
   this.gestionServicioBW = gestionServicioBW;
        }

        [HttpPost("RegistrarServicio")]
     public async Task<ActionResult> RegistrarServicio([FromBody] Servicio servicio)
     {
            try
    {
     var resultado = await gestionServicioBW.registrarServicio(servicio);
        if (resultado)
     return Ok(new { success = true, message = "Servicio registrado exitosamente" });

 return BadRequest(new { success = false, message = "No se pudo registrar el servicio" });
  }
     catch (Exception ex)
  {
        return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
         }
}

        [HttpGet("ObtenerTodosLosServicios")]
        public async Task<ActionResult> ObtenerTodosLosServicios()
    {
   try
{
   var resultado = await gestionServicioBW.obtenerTodosLosServicios();
      return Ok(new { success = true, data = resultado });
         }
    catch (Exception ex)
    {
     return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
  }
   }

   [HttpGet("ObtenerServicioPorId/{idServicio}")]
    public async Task<ActionResult> ObtenerServicioPorId(int idServicio)
    {
try
 {
     var resultado = await gestionServicioBW.obtenerServicioPorId(idServicio);
 if (resultado == null)
     return NotFound(new { success = false, message = "Servicio no encontrado" });

      return Ok(new { success = true, data = resultado });
  }
    catch (Exception ex)
        {
    return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut("ActualizarServicio/{idServicio}")]
      public async Task<ActionResult> ActualizarServicio([FromBody] Servicio servicio, int idServicio)
{
try
         {
    var resultado = await gestionServicioBW.actualizarServicio(servicio, idServicio);
    if (resultado)
       return Ok(new { success = true, message = "Servicio actualizado exitosamente" });

      return BadRequest(new { success = false, message = "No se pudo actualizar el servicio" });
  }
  catch (Exception ex)
   {
return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
   }
     }

     [HttpDelete("EliminarServicio/{idServicio}")]
        public async Task<ActionResult> EliminarServicio(int idServicio)
     {
        try
       {
 var resultado = await gestionServicioBW.eliminarServicio(idServicio);
      if (resultado)
   return Ok(new { success = true, message = "Servicio eliminado exitosamente" });

        return BadRequest(new { success = false, message = "No se pudo eliminar el servicio" });
 }
         catch (Exception ex)
 {
      return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
    }
        }
}
}
