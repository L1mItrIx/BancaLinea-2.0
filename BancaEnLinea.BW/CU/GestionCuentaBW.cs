using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
    public class GestionCuentaBW : IGestionCuentaBW
    {
        private readonly IGestionCuentaDA gestionCuentaDA;
        public GestionCuentaBW(IGestionCuentaDA gestionCuentaDA)
        {
            this.gestionCuentaDA = gestionCuentaDA;
        }

        public async Task<bool> registrarCuenta(Cuenta cuenta)
        {
            if (!ReglasDeCuenta.laCuentaEsValida(cuenta))
            {
                return false;
            }

            var todasLasCuentas = await gestionCuentaDA.obtenerCuentas();
            //coreo unico
            bool correoExiste = todasLasCuentas.Any(c =>
              c.Correo.ToLower().Trim() == cuenta.Correo.ToLower().Trim());
            if (correoExiste)
            {
                return false;
            }

            return await gestionCuentaDA.registrarCuenta(cuenta);
        }

        public Task<bool> actualizarCuenta(Cuenta cuenta, int id)
        {
            if (!ReglasDeCuenta.elIdEsValido(id) || !ReglasDeCuenta.laCuentaEsValida(cuenta))
            {
                return Task.FromResult(false);
            }
            return gestionCuentaDA.actualizarCuenta(cuenta, id);
        }

        public Task<bool> eliminarCuenta(int id)
        {
            return ReglasDeCuenta.elIdEsValido(id) ?
              gestionCuentaDA.eliminarCuenta(id) :
              Task.FromResult(false);
        }

        public Task<List<Cuenta>> obtenerCuentas()
        {
            return gestionCuentaDA.obtenerCuentas();
        }

        public Task<Cuenta?> validarCuenta(string correoElectronico, string contrasena)
        {
            if (!ReglasDeCuenta.elCorreoEsValido(correoElectronico) || string.IsNullOrEmpty(contrasena))
            {
                return Task.FromResult<Cuenta?>(null);
            }
            return gestionCuentaDA.validarCuenta(correoElectronico, contrasena);
        }
    }
}