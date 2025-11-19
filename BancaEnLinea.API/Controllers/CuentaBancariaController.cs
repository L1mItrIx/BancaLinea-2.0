using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Mvc;

namespace BancaEnLinea.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CuentasBancariasController : ControllerBase
    {
        private readonly IGestionCuentaBancariaBW gestionCuentaBancariaBW;

        public CuentasBancariasController(IGestionCuentaBancariaBW gestionCuentaBancariaBW)
        {
            this.gestionCuentaBancariaBW = gestionCuentaBancariaBW;
        }

        [HttpPost("RegistrarCuentaBancaria")]
        public async Task<ActionResult> RegistrarCuentaBancaria([FromBody] CuentaBancaria cuentaBancaria, [FromQuery] int idCuenta)
        {
            try
            {
                var resultado = await gestionCuentaBancariaBW.registrarCuentaBancaria(cuentaBancaria, idCuenta);
                if (resultado)
                    return Ok(new { success = true, message = "Cuenta bancaria registrada exitosamente" });

                return BadRequest(new { success = false, message = "No se pudo registrar la cuenta bancaria" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerTodasLasCuentasBancarias")]
        public async Task<ActionResult> ObtenerTodasLasCuentasBancarias()
        {
            try
            {
                var resultado = await gestionCuentaBancariaBW.obtenerTodasLasCuentasBancarias();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerCuentaBancaria/{id}")]
        public async Task<ActionResult> ObtenerCuentaBancariaPorId(int id)
        {
            try
            {
                var resultado = await gestionCuentaBancariaBW.obtenerCuentaBancariaPorId(id);
                if (resultado == null)
                    return NotFound(new { message = "Cuenta bancaria no encontrada" });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
            }
        }

        [HttpPut("ActualizarCuentaBancaria/{id}")]
        public async Task<ActionResult> ActualizarCuentaBancaria([FromBody] CuentaBancaria cuentaBancaria, int id)
        {
            try
            {
                var resultado = await gestionCuentaBancariaBW.actualizarCuentaBancaria(cuentaBancaria, id);
                if (resultado)
                    return Ok(new { success = true, message = "Cuenta bancaria actualizada exitosamente" });

                return BadRequest(new { success = false, message = "No se pudo actualizar la cuenta bancaria" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
            }
        }

        [HttpDelete("EliminarCuentaBancaria/{id}")]
        public async Task<ActionResult> EliminarCuentaBancaria(int id)
        {
            try
            {
                var resultado = await gestionCuentaBancariaBW.eliminarCuentaBancaria(id);
                if (resultado)
                    return Ok(new { success = true, message = "Cuenta bancaria eliminada exitosamente" });

                return BadRequest(new { success = false, message = "No se pudo eliminar la cuenta bancaria" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
            }
        }
    }
}