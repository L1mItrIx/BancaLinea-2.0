using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
    public class GestionPagoServicioBW : IGestionPagoServicioBW
    {
        private readonly IGestionPagoServicioDA gestionPagoServicioDA;
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;
        private readonly IGestionContratoServicioDA gestionContratoServicioDA;

        public GestionPagoServicioBW(
            IGestionPagoServicioDA gestionPagoServicioDA,
            IGestionCuentaBancariaDA gestionCuentaBancariaDA,
            IGestionContratoServicioDA gestionContratoServicioDA)
        {
            this.gestionPagoServicioDA = gestionPagoServicioDA;
            this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
            this.gestionContratoServicioDA = gestionContratoServicioDA;
        }

        public async Task<(bool exito, string mensaje, int? idPago)> realizarPago(PagoServicioRequest request)
        {
            var validacion = await validarDatosPago(request);
            if (!validacion.esValido)
                return (false, validacion.mensaje, null);

            var montos = calcularMontosConComision(request.Monto, validacion.cuentaBancaria.Moneda);

            var validacionSaldo = validarSaldoSuficiente(validacion.cuentaBancaria, montos.montoTotal);
            if (!validacionSaldo.esValido)
                return (false, validacionSaldo.mensaje, null);

            var pago = crearPago(request, validacion.contrato, montos);

            int idPago = await gestionPagoServicioDA.registrarPago(pago);
            if (idPago <= 0)
                return (false, "Error al registrar el pago", null);

            return await procesarPagoSegunEstado(pago, idPago, validacion.cuentaBancaria);
        }

        private async Task<(bool esValido, string mensaje, ContratoServicio contrato, CuentaBancaria cuentaBancaria)>
            validarDatosPago(PagoServicioRequest request)
        {
            var contrato = await gestionContratoServicioDA.obtenerContratoPorNumero(request.NumeroContrato);
            if (contrato == null)
                return (false, "Número de contrato no encontrado", null, null);

            var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(request.IdCuentaBancariaOrigen);
            if (cuentaBancaria == null)
                return (false, "Cuenta bancaria no encontrada", null, null);

            if (cuentaBancaria.Estado != EstadoCB.Activa)
                return (false, "La cuenta bancaria no está activa", null, null);

            return (true, string.Empty, contrato, cuentaBancaria);
        }

        private (long comision, long montoTotal) calcularMontosConComision(long monto, Moneda monedaCuenta)
        {
            const long COMISION_PAGO_SERVICIO_CRC = 300;

            long comision = monedaCuenta == Moneda.CRC
                ? COMISION_PAGO_SERVICIO_CRC
                : ReglasDeConversionMoneda.convertirMoneda(COMISION_PAGO_SERVICIO_CRC, Moneda.CRC, Moneda.USD);

            long montoTotal = monto + comision;

            return (comision, montoTotal);
        }

        private (bool esValido, string mensaje) validarSaldoSuficiente(CuentaBancaria cuentaBancaria, long montoTotal)
        {
            if (cuentaBancaria.Saldo < montoTotal)
                return (false, "Saldo insuficiente");

            return (true, string.Empty);
        }

        private PagoServicio crearPago(PagoServicioRequest request, ContratoServicio contrato, (long comision, long montoTotal) montos)
        {
            DateTime fechaEjecucion = request.FechaEjecucion ?? DateTime.Now;
            EstadoTra estado = determinarEstadoPago(fechaEjecucion);

            return new PagoServicio
            {
                IdContratoServicio = contrato.IdContratoServicio,
                IdCuentaBancariaOrigen = request.IdCuentaBancariaOrigen,
                Monto = request.Monto,
                Comision = montos.comision,
                MontoTotal = montos.montoTotal,
                FechaCreacion = DateTime.Now,
                FechaEjecucion = fechaEjecucion,
                Estado = estado
            };
        }

        private EstadoTra determinarEstadoPago(DateTime fechaEjecucion)
        {
            return fechaEjecucion.Date > DateTime.Now.Date
                ? EstadoTra.Programada
                : EstadoTra.Pendiente;
        }

        private async Task<(bool exito, string mensaje, int? idPago)> procesarPagoSegunEstado(
            PagoServicio pago, int idPago, CuentaBancaria cuentaBancaria)
        {
            if (pago.Estado == EstadoTra.Pendiente)
            {
                await ejecutarPagoInmediato(pago, idPago, cuentaBancaria);
                return (true, "Pago realizado exitosamente", idPago);
            }

            return (true, $"Pago programado para {pago.FechaEjecucion:dd/MM/yyyy}", idPago);
        }

        private async Task ejecutarPagoInmediato(PagoServicio pago, int idPago, CuentaBancaria cuentaBancaria)
        {
            cuentaBancaria.Saldo -= pago.MontoTotal;
            cuentaBancaria.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancaria.Saldo, cuentaBancaria.Estado);

            await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancaria, cuentaBancaria.Id);
            await gestionPagoServicioDA.actualizarEstado(idPago, (int)EstadoTra.Exitosa);
        }

        public Task<List<PagoServicio>> obtenerPagosPorCliente(int idCliente)
        {
            return gestionPagoServicioDA.obtenerPagosPorCliente(idCliente);
        }

        public Task<List<PagoServicio>> obtenerPagosProgramados(int idCliente)
        {
            return gestionPagoServicioDA.obtenerPagosProgramados(idCliente);
        }

        public Task<List<PagoServicio>> obtenerTodosPagos()
        {
            return gestionPagoServicioDA.obtenerTodosPagos();
        }

        public async Task<(bool exito, string mensaje)> cancelarPago(int idPago, int idCliente)
        {
            var pago = await gestionPagoServicioDA.obtenerPagoPorId(idPago);
            if (pago == null)
                return (false, "Pago no encontrado");

            // Validar que sea del cliente
            var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(pago.IdCuentaBancariaOrigen);
            if (cuentaBancaria?.IdCuenta != idCliente)
                return (false, "No tiene permisos para cancelar este pago");

            // Validar que sea programado
            if (pago.Estado != EstadoTra.Programada)
                return (false, "Solo se pueden cancelar pagos programados");

            // Validar 24 horas
            TimeSpan diferencia = pago.FechaEjecucion - DateTime.Now;
            if (diferencia.TotalHours < 24)
                return (false, "Solo se puede cancelar con al menos 24 horas de anticipación");

            bool resultado = await gestionPagoServicioDA.cancelarPago(idPago);
            return resultado
                ? (true, "Pago cancelado exitosamente")
                : (false, "Error al cancelar el pago");
        }
    }
}
