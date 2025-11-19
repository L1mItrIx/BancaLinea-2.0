using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BeneficiariosController : ControllerBase
    {
private readonly IGestionBeneficiarioBW gestionBeneficiarioBW;

      public BeneficiariosController(IGestionBeneficiarioBW gestionBeneficiarioBW)
        {
            this.gestionBeneficiarioBW = gestionBeneficiarioBW;
        }

   [HttpPost("RegistrarBeneficiario")]
public async Task<ActionResult> RegistrarBeneficiario([FromBody] Beneficiarios beneficiario)
        {
          try
     {
   var resultado = await gestionBeneficiarioBW.registrarBeneficiario(beneficiario);
    if (resultado)
            return Ok(new { success = true, message = "Beneficiario registrado exitosamente" });

  return BadRequest(new { success = false, message = "No se pudo registrar el beneficiario. Verifique los datos o el límite de 3 beneficiarios por cliente." });
       }
            catch (Exception ex)
            {
           return StatusCode(500, new { success = false, message = $"Error interno en el servidor: {ex.Message}" });
       }
        }

        [HttpGet("ObtenerBeneficiariosPorCliente/{idCuenta}")]
        public async Task<ActionResult> ObtenerBeneficiariosPorCliente(int idCuenta)
        {
       try
          {
             var resultado = await gestionBeneficiarioBW.obtenerBeneficiariosPorCliente(idCuenta);
     return Ok(new { success = true, data = resultado });
    }
    catch (Exception ex)
            {
       return StatusCode(500, new { success = false, message = $"Error interno en el servidor: {ex.Message}" });
  }
        }

        [HttpGet("ObtenerTodosLosBeneficiarios")]
        public async Task<ActionResult> ObtenerTodosLosBeneficiarios()
        {
    try
            {
      var resultado = await gestionBeneficiarioBW.obtenerTodosLosBeneficiarios();
  return Ok(new { success = true, data = resultado });
            }
        catch (Exception ex)
      {
     return StatusCode(500, new { success = false, message = $"Error interno en el servidor: {ex.Message}" });
  }
     }

     [HttpGet("ObtenerBeneficiarioPorId/{idBeneficiario}")]
        public async Task<ActionResult> ObtenerBeneficiarioPorId(int idBeneficiario)
        {
            try
 {
                var resultado = await gestionBeneficiarioBW.obtenerBeneficiarioPorId(idBeneficiario);
      if (resultado == null)
         return NotFound(new { success = false, message = "Beneficiario no encontrado" });

     return Ok(new { success = true, data = resultado });
 }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error interno en el servidor: {ex.Message}" });
    }
     }

        [HttpPut("ActualizarBeneficiario/{idBeneficiario}")]
     public async Task<ActionResult> ActualizarBeneficiario([FromBody] Beneficiarios beneficiario, int idBeneficiario)
        {
 try
      {
     var resultado = await gestionBeneficiarioBW.actualizarBeneficiario(beneficiario, idBeneficiario);
   if (resultado)
          return Ok(new { success = true, message = "Beneficiario actualizado exitosamente" });

 return BadRequest(new { success = false, message = "No se pudo actualizar el beneficiario" });
          }
  catch (Exception ex)
            {
          return StatusCode(500, new { success = false, message = $"Error interno en el servidor: {ex.Message}" });
    }
        }

        [HttpDelete("EliminarBeneficiario/{idBeneficiario}")]
     public async Task<ActionResult> EliminarBeneficiario(int idBeneficiario)
        {
        try
            {
    var resultado = await gestionBeneficiarioBW.eliminarBeneficiario(idBeneficiario);
     if (resultado)
  return Ok(new { success = true, message = "Beneficiario eliminado exitosamente" });

    return BadRequest(new { success = false, message = "No se pudo eliminar el beneficiario" });
   }
            catch (Exception ex)
            {
 return StatusCode(500, new { success = false, message = $"Error interno en el servidor: {ex.Message}" });
       }
        }
    }
}
