using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
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

        [HttpGet("ObtenerCuentasBancarias/{idCuenta}")]
        public async Task<ActionResult> ObtenerCuentasBancarias(int idCuenta)
        {
            try
            {
                var cuentas = await gestionCuentaBancariaBW.obtenerCuentasBancarias(idCuenta);
                var cuentasConInfo = mapearCuentasConInformacion(cuentas);
                return Ok(cuentasConInfo);
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
                var cuentas = await gestionCuentaBancariaBW.obtenerTodasLasCuentasBancarias();
                var cuentasConInfo = mapearCuentasConInformacion(cuentas);
                return Ok(cuentasConInfo);
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

        private List<object> mapearCuentasConInformacion(List<CuentaBancaria> cuentas)
        {
            return cuentas.Select(c => mapearCuentaBancariaConInfo(c)).ToList();
        }

        private object mapearCuentaBancariaConInfo(CuentaBancaria c)
        {
            return new
            {
                id = c.Id,
                numeroTarjeta = c.NumeroTarjeta,
                tipo = c.Tipo,
                tipoTexto = ObtenerTipoTexto(c.Tipo),
                moneda = c.Moneda,
                monedaTexto = c.Moneda == Moneda.CRC ? "CRC" : "USD",
                simboloMoneda = c.Moneda == Moneda.CRC ? "₡" : "$",
                saldo = c.Saldo,
                estado = c.Estado,
                estadoTexto = ObtenerEstadoTexto(c.Estado),
                idCuenta = c.IdCuenta,
                nombreDueno = obtenerNombreCompletoDueno(c.Cuenta)
            };
        }

        private string obtenerNombreCompletoDueno(Cuenta cuenta)
        {
            if (cuenta == null)
                return null;

            return $"{cuenta.Nombre} {cuenta.PrimerApellido} {cuenta.SegundoApellido}";
        }

        private string ObtenerTipoTexto(TipoCuenta tipo)
        {
            return tipo switch
            {
                TipoCuenta.Ahorros => "Ahorros",
                TipoCuenta.Corriente => "Corriente",
                TipoCuenta.Inversion => "Inversión",
                TipoCuenta.PlazoFijo => "Plazo Fijo",
                _ => "Desconocido"
            };
        }

        private string ObtenerEstadoTexto(EstadoCB estado)
        {
            return estado switch
            {
                EstadoCB.Activa => "Activa",
                EstadoCB.Bloqueada => "Bloqueada",
                EstadoCB.Cerrada => "Cerrada",
                _ => "Desconocido"
            };
        }
    }
}