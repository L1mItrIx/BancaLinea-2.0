using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.DA
{
    public interface IGestionCuentaDA
    {
        Task<bool> registrarCuenta(Cuenta cuenta);
        Task<List<Cuenta>> obtenerCuentas();
        Task<bool> actualizarCuenta(Cuenta cuenta, int id);
        Task<bool> eliminarCuenta(int id);
        Task<Cuenta?> validarCuenta(string correo, string contrasena);
        Task<Cuenta?> obtenerCuentaPorId(int id);
  }
}
