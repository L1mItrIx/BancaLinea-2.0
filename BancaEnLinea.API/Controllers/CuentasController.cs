using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
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
                Console.WriteLine($"?? Intento de login: {loginRequest.Correo}");

                var cuentaUsuario = await gestionCuentaBW.validarCuenta(loginRequest.Correo, loginRequest.Contrasena);

                if (cuentaUsuario != null)
                {
                    // Determinar el nombre del rol y la ruta de redirección
                    string rolNombre;
                    string rutaRedireccion;

                    switch (cuentaUsuario.Rol)
                    {
                        case RolCuenta.Administrador:
                            rolNombre = "Administrador";
                            rutaRedireccion = "/admin/dashboard";
                            Console.WriteLine($"? Login exitoso - Administrador: {cuentaUsuario.Nombre}");
                            break;
                        case RolCuenta.Gestor:
                            rolNombre = "Gestor";
                            rutaRedireccion = "/gestor/dashboard";
                            Console.WriteLine($"? Login exitoso - Gestor: {cuentaUsuario.Nombre}");
                            break;
                        case RolCuenta.Cliente:
                            rolNombre = "Cliente";
                            rutaRedireccion = "/cliente/dashboard";
                            Console.WriteLine($"? Login exitoso - Cliente: {cuentaUsuario.Nombre}");
                            break;
                        default:
                            rolNombre = "Desconocido";
                            rutaRedireccion = "/";
                            Console.WriteLine($"?? Login con rol desconocido: {cuentaUsuario.Rol}");
                            break;
                    }

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
                            rol = cuentaUsuario.Rol,
                            rolNombre = rolNombre,
                            nombreCompleto = $"{cuentaUsuario.Nombre} {cuentaUsuario.PrimerApellido} {cuentaUsuario.SegundoApellido}",
                            rutaRedireccion = rutaRedireccion
                        }
                    });
                }

                Console.WriteLine($"? Login fallido para: {loginRequest.Correo}");
                return Unauthorized(new
                {
                    success = false,
                    message = "Correo o contraseña incorrectos"
                });
            }
            catch (Exception excepcion)
            {
                Console.WriteLine($"?? ERROR en login: {excepcion.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error interno: {excepcion.Message}"
                });
            }
        }
    /// <summary>
    /// Obtiene SOLO las cuentas de clientes (Rol = Cliente).
    /// Los gestores usan este endpoint para no ver otros gestores o administradores.
    /// </summary>
    [HttpGet("ObtenerCuentasClientes")]
    public async Task<ActionResult> ObtenerCuentasClientes()
    {
      try
      {
        Console.WriteLine("?? Obteniendo cuentas de clientes...");

        var todasLasCuentas = await gestionCuentaBW.obtenerCuentas();

        // Filtrar SOLO clientes (Rol = 2)
        var soloClientes = todasLasCuentas
          .Where(cuenta => cuenta.Rol == RolCuenta.Cliente)
          .Select(cuenta => new
          {
            id = cuenta.Id,
            telefono = cuenta.Telefono,
            nombre = cuenta.Nombre,
            primerApellido = cuenta.PrimerApellido,
            segundoApellido = cuenta.SegundoApellido,
            correo = cuenta.Correo,
            rol = cuenta.Rol,
            nombreCompleto = $"{cuenta.Nombre} {cuenta.PrimerApellido} {cuenta.SegundoApellido}"
          })
          .ToList();

        Console.WriteLine($"? Se encontraron {soloClientes.Count} clientes");

        return Ok(new
        {
          success = true,
          data = soloClientes,
          total = soloClientes.Count
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"? ERROR: {ex.Message}");
        return StatusCode(500, new
        {
          success = false,
          message = $"Error interno en el servidor: {ex.Message}"
        });
      }
    }

    /// <summary>
    /// Obtiene información básica de una cuenta cliente por su ID.
    /// Valida que sea un cliente antes de devolver la información.
    /// </summary>
    [HttpGet("ObtenerCuentaCliente/{id}")]
    public async Task<ActionResult> ObtenerCuentaCliente(int id)
    {
      try
      {
        Console.WriteLine($"?? Buscando cliente con ID: {id}");

        var todasLasCuentas = await gestionCuentaBW.obtenerCuentas();
        var cliente = todasLasCuentas.FirstOrDefault(c => c.Id == id && c.Rol == RolCuenta.Cliente);

        if (cliente == null)
        {
          Console.WriteLine($"? No se encontró cliente con ID: {id}");
          return NotFound(new
          {
            success = false,
            message = "Cliente no encontrado o no es un cliente válido"
          });
        }

        Console.WriteLine($"? Cliente encontrado: {cliente.Nombre} {cliente.PrimerApellido}");

        return Ok(new
        {
          success = true,
          data = new
          {
            id = cliente.Id,
            telefono = cliente.Telefono,
            nombre = cliente.Nombre,
            primerApellido = cliente.PrimerApellido,
            segundoApellido = cliente.SegundoApellido,
            correo = cliente.Correo,
            rol = cliente.Rol,
            nombreCompleto = $"{cliente.Nombre} {cliente.PrimerApellido} {cliente.SegundoApellido}"
          }
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"? ERROR: {ex.Message}");
        return StatusCode(500, new
        {
          success = false,
          message = $"Error interno en el servidor: {ex.Message}"
        });
      }
    }
  }
}
