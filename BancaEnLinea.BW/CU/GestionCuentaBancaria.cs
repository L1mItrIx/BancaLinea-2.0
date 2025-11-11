using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
    public class GestionCuentaBancariaBW : IGestionCuentaBancariaBW
    {
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;

        public GestionCuentaBancariaBW(IGestionCuentaBancariaDA gestionCuentaBancariaDA)
        {
            this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
        }

        public async Task<bool> registrarCuentaBancaria(CuentaBancaria cuentaBancaria, int idCuenta)
        {
            if (!ReglasDeCuentaBancaria.laCuentaBancariaEsValida(cuentaBancaria) || !ReglasDeCuenta.elIdEsValido(idCuenta))
            {
                return false;
            }

            // Obtener cuentas existentes del usuario
            var cuentasExistentes = await gestionCuentaBancariaDA.obtenerCuentasBancarias(idCuenta);

            // Validar si puede crear nueva cuenta
            if (!ReglasDeCuentaBancaria.puedeCrearNuevaCuenta(cuentasExistentes, cuentaBancaria))
            {
                return false;
            }

            return await gestionCuentaBancariaDA.registrarCuentaBancaria(cuentaBancaria, idCuenta);
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

        public Task<bool> actualizarCuentaBancaria(CuentaBancaria cuentaBancaria, int id)
        {
            if (!ReglasDeCuenta.elIdEsValido(id) || !ReglasDeCuentaBancaria.laCuentaBancariaEsValida(cuentaBancaria))
            {
                return Task.FromResult(false);
            }
            return gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancaria, id);
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