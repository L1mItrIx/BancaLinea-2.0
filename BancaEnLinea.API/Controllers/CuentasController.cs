using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace BancaEnLinea.API.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class CuentasController : ControllerBase
  {
    private readonly IGestionCuentaBW gestionCuentaBW;

    public CuentasController(IGestionCuentaBW gestionCuentaBW)
    {
      this.gestionCuentaBW = gestionCuentaBW;
    }
    [HttpPost("RegistrarCuenta")]
    public async Task<ActionResult> registrarCuenta([FromBody] Cuenta cuenta)
    {
      var resultado = await gestionCuentaBW.registrarCuenta(cuenta);
      try
      {
        return Ok(resultado);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
      }
    }
    [HttpGet("ObtenerCuentas")]
    public async  Task<ActionResult> obtenerCuentas()
    {
      var resultado = await gestionCuentaBW.obtenerCuentas();
      try
      {
        return Ok(resultado);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
      }
    }
    [HttpPut("{id}", Name ="ActualizarCuenta")]
    public async Task<ActionResult> actualizarCuenta([FromBody] Cuenta cuenta, int id)
    {
      var resultado = await gestionCuentaBW.actualizarCuenta(cuenta, id);
      try
      {
        return Ok(resultado);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
      }
    }
    [HttpDelete("{id}", Name = "EliminarCuenta")]
    public async Task<ActionResult> eliminarCuenta(int id)
    {
      var resultado = await gestionCuentaBW.eliminarCuenta(id);
      try
      {
        return Ok(resultado);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
      }
    }
    [HttpPost("ValidarCuenta")]
    public async Task<ActionResult> validarCuenta([FromBody] Cuenta cuenta)
    {
      var resultado = await gestionCuentaBW.validarCuenta(cuenta.Correo, cuenta.Contrasena);
      try
      {
        return Ok(resultado);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
      }
    }
  }
}
