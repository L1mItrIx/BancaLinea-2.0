using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BC.Enums;
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
        public async Task<ActionResult> RegistrarBeneficiario([FromBody] BeneficiarioRequest request)
    {
   try
 {
        Console.WriteLine($"?? Intentando registrar beneficiario: {request.Alias} para cliente {request.IdCuenta}");
  
    // Mapear de BeneficiarioRequest a Beneficiarios (modelo completo)
                var beneficiario = new Beneficiarios
     {
 IdCuenta = request.IdCuenta,
 Alias = request.Alias,
   Banco = request.Banco,
         Moneda = request.Moneda,
     NumeroCuentaDestino = request.NumeroCuentaDestino,
    Pais = request.Pais,
   Estado = EstadoP.Pendiente  // Siempre empieza como Pendiente
          };

     var resultado = await gestionBeneficiarioBW.registrarBeneficiario(beneficiario);
   if (resultado)
            {
      Console.WriteLine($"? Beneficiario registrado exitosamente");
      return Ok(new { success = true, message = "Beneficiario registrado exitosamente" });
      }

       Console.WriteLine($"? No se pudo registrar el beneficiario (ver logs arriba para detalles)");
                return BadRequest(new { success = false, message = "No se pudo registrar el beneficiario. Verifique los datos o el límite de 3 beneficiarios por cliente." });
        }
        catch (Exception ex)
    {
        Console.WriteLine($"?? ERROR INESPERADO: {ex.Message}");
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
  return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
     }
   }

     [HttpGet("ObtenerBeneficiariosPendientes")]
    public async Task<ActionResult> ObtenerBeneficiariosPendientes()
  {
       try
  {
    var resultado = await gestionBeneficiarioBW.obtenerBeneficiariosPendientes();
       return Ok(new { success = true, data = resultado });
   }
   catch (Exception ex)
    {
  return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
  }
   }

  [HttpPut("ConfirmarBeneficiario/{idBeneficiario}")]
        public async Task<ActionResult> ConfirmarBeneficiario(int idBeneficiario)
  {
   try
   {
 var (exito, mensaje) = await gestionBeneficiarioBW.confirmarBeneficiario(idBeneficiario);
     
       if (exito)
       return Ok(new { success = true, message = mensaje });

   return BadRequest(new { success = false, message = mensaje });
   }
  catch (Exception ex)
        {
       return StatusCode(500, new { success = false, message = $"Error interno: {ex.Message}" });
 }
    }

   [HttpPut("RechazarBeneficiario/{idBeneficiario}")]
  public async Task<ActionResult> RechazarBeneficiario(int idBeneficiario)
   {
        try
  {
     var (exito, mensaje) = await gestionBeneficiarioBW.rechazarBeneficiario(idBeneficiario);
      
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
