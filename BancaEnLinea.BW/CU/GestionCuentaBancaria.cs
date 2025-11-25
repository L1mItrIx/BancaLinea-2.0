using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BW.CU
{
    public class GestionCuentaBancariaBW : IGestionCuentaBancariaBW
    {
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;
        private readonly IGestionCuentaDA gestionCuentaDA;

        public GestionCuentaBancariaBW(IGestionCuentaBancariaDA gestionCuentaBancariaDA, IGestionCuentaDA gestionCuentaDA)
        {
            this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
            this.gestionCuentaDA = gestionCuentaDA;
        }

        public async Task<bool> registrarCuentaBancaria(CuentaBancaria cuentaBancaria, int idCuenta)
        {
            var cuenta = await gestionCuentaDA.obtenerCuentaPorId(idCuenta);
            if (!validarCuentaCliente(cuenta))
                return false;

            await prepararCuentaBancaria(cuentaBancaria);

            if (!ReglasDeCuentaBancaria.laCuentaBancariaEsValida(cuentaBancaria))
                return false;

            var cuentasExistentes = await gestionCuentaBancariaDA.obtenerCuentasBancarias(idCuenta);
            if (!ReglasDeCuentaBancaria.puedeCrearNuevaCuenta(cuentasExistentes, cuentaBancaria))
                return false;

            return await gestionCuentaBancariaDA.registrarCuentaBancaria(cuentaBancaria, idCuenta);
        }

        private bool validarCuentaCliente(Cuenta cuenta)
        {
            return cuenta != null && ReglasDeCuentaBancaria.puedeTenerCuentaBancaria(cuenta);
        }

        private async Task prepararCuentaBancaria(CuentaBancaria cuentaBancaria)
        {
            cuentaBancaria.NumeroTarjeta = await GenerarNumeroTarjetaUnico();
            cuentaBancaria.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancaria.Saldo, cuentaBancaria.Estado);
        }

        private async Task<long> GenerarNumeroTarjetaUnico()
        {
            int intentos = 250;
            for (int i = 0; i < intentos; i++)
            {
                long numeroTarjeta = GenerarNumeroAleatorio12Digitos();
                var todasLasCuentas = await gestionCuentaBancariaDA.obtenerTodasLasCuentasBancarias();
                bool existe = todasLasCuentas.Any(x => x.NumeroTarjeta == numeroTarjeta);

                if (!existe)
                { return numeroTarjeta; }
            }
            throw new Exception("No se pudo generar un número de tarjeta único después de varios intentos.");
        }
        private long GenerarNumeroAleatorio12Digitos()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            long min = 100000000000;
            long max = 999999999999;


            long numeroTarjeta = (long)(random.NextDouble() * (max - min) + min);

            return numeroTarjeta;
        }

        public Task<List<CuentaBancaria>> obtenerCuentasBancarias(int idCuenta)
        {
            return ReglasDeCuenta.elIdEsValido(idCuenta) ?
              gestionCuentaBancariaDA.obtenerCuentasBancarias(idCuenta) :
              Task.FromResult(new List<CuentaBancaria>());
        }

        public Task<List<CuentaBancaria>> obtenerTodasLasCuentasBancarias()
        {
            return gestionCuentaBancariaDA.obtenerTodasLasCuentasBancarias();
        }

        public async Task<bool> actualizarCuentaBancaria(CuentaBancaria cuentaBancaria, int id)
        {
            if (!ReglasDeCuenta.elIdEsValido(id))
                return false;

            var cuentaExistente = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(id);
            if (cuentaExistente == null)
                return false;

            await configurarCuentaBancariaParaActualizacion(cuentaBancaria, cuentaExistente);

            if (!ReglasDeCuentaBancaria.laCuentaBancariaEsValida(cuentaBancaria))
                return false;

            return await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancaria, id);
        }

        private async Task configurarCuentaBancariaParaActualizacion(CuentaBancaria cuentaBancaria, CuentaBancaria cuentaExistente)
        {
            cuentaBancaria.NumeroTarjeta = cuentaExistente.NumeroTarjeta;

            if (esNumeroTarjetaInvalido(cuentaBancaria.NumeroTarjeta))
                cuentaBancaria.NumeroTarjeta = await GenerarNumeroTarjetaUnico();

            cuentaBancaria.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancaria.Saldo, cuentaBancaria.Estado);
        }

        private bool esNumeroTarjetaInvalido(long numeroTarjeta)
        {
            return numeroTarjeta == 0 || numeroTarjeta.ToString().Length != 12;
        }

        public Task<bool> eliminarCuentaBancaria(int id)
        {
            return ReglasDeCuenta.elIdEsValido(id) ?
              gestionCuentaBancariaDA.eliminarCuentaBancaria(id) :
              Task.FromResult(false);
        }

        public Task<CuentaBancaria?> obtenerCuentaBancariaPorId(int id)
        {
            return ReglasDeCuenta.elIdEsValido(id) ?
              gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(id) :
              Task.FromResult<CuentaBancaria?>(null);
        }
    }
}