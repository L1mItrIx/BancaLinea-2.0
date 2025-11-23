using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
  [Route("[controller]")]
  [ApiController]
    public class AccionesController : ControllerBase
    {
        private readonly IGestionAccionBW gestionAccionBW;

        public AccionesController(IGestionAccionBW gestionAccionBW)
        {
   this.gestionAccionBW = gestionAccionBW;
 }

 /// <summary>
   /// Registra una acción en el sistema
  /// </summary>
  [HttpPost("RegistrarAccion")]
        public async Task<ActionResult> RegistrarAccion([FromBody] Accion accion)
 {
   try
 {
       var resultado = await gestionAccionBW.registrarAccion(accion);

          if (resultado)
       return Ok(new { success = true, message = "Acción registrada exitosamente" });

    return BadRequest(new { success = false, message = "No se pudo registrar la acción" });
       }
   catch (Exception ex)
 {
        return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
   }
     }

   /// <summary>
   /// Obtiene todas las acciones registradas
   /// </summary>
   [HttpGet("ObtenerAcciones")]
        public async Task<ActionResult> ObtenerAcciones()
 {
      try
      {
         var acciones = await gestionAccionBW.obtenerTodasLasAcciones();

 return Ok(new
     {
      success = true,
        data = acciones,
       total = acciones.Count
     });
    }
 catch (Exception ex)
   {
          return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
     }
 }
    }
}
