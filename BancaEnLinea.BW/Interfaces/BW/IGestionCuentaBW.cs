using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
  public interface IGestionCuentaBW
  {
    Task<bool> registrarCuenta(Cuenta cuenta);
    Task<List<Cuenta>> obtenerCuentas();
    Task<bool> actualizarCuenta(Cuenta cuenta, int id);
    Task<bool> eliminarCuenta(int id);
    Task<bool> validarCuenta(string correo, string contrasena);
  }
}
