using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
    [HttpPut("ActualizarCuenta/{id}")]
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
    [HttpDelete("EliminarCuenta/{id}")]
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
        public async Task<ActionResult> validarCuenta([FromBody] BancaEnLinea.BC.Modelos.LoginRequest loginRequest)
        {
            try
            {
                var cuentaUsuario = await gestionCuentaBW.validarCuenta(loginRequest.Correo, loginRequest.Contrasena);

                if (cuentaUsuario != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Inicio de sesión exitoso",
                        data = new
                        {
                            id = cuentaUsuario.Id,
                            telefono = cuentaUsuario.Telefono,
                            nombre = cuentaUsuario.Nombre,
                            primerApellido = cuentaUsuario.PrimerApellido,
                            segundoApellido = cuentaUsuario.SegundoApellido,
                            correo = cuentaUsuario.Correo,
                            rol = cuentaUsuario.Rol
                        }
                    });
                }

                return Unauthorized(new
                {
                    success = false,
                    message = "Correo o contraseña incorrectos"
                });
            }
            catch (Exception excepcion)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error interno: {excepcion.Message}"
                });
            }
        }
    }
}
