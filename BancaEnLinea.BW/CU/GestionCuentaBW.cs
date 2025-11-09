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

    public Task<bool> actualizarCuenta(Cuenta cuenta, int id)
    {
      if(!ReglasDeCuenta.elIdEsValido(id) || !ReglasDeCuenta.laCuentaEsValida(cuenta))
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

    public Task<bool> registrarCuenta(Cuenta cuenta)
    {
      return ReglasDeCuenta.laCuentaEsValida(cuenta) ?
        gestionCuentaDA.registrarCuenta(cuenta) :
        Task.FromResult(false);
    }

    public Task<bool> validarCuenta(string correo, string contrasena)
    {
      if (!ReglasDeCuenta.elCorreoEsValido(correo) || string.IsNullOrEmpty(contrasena))
      {
        return Task.FromResult(false);
      }
      return gestionCuentaDA.validarCuenta(correo, contrasena);
    }
  }
}
