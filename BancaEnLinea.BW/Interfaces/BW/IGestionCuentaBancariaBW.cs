using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionCuentaBancariaBW
    {
        Task<bool> registrarCuentaBancaria(CuentaBancaria cuentaBancaria, int idCuenta);
        Task<List<CuentaBancaria>> obtenerCuentasBancarias(int idCuenta);
        Task<List<CuentaBancaria>> obtenerTodasLasCuentasBancarias();
        Task<bool> actualizarCuentaBancaria(CuentaBancaria cuentaBancaria, int id);
        Task<bool> eliminarCuentaBancaria(int id);
        Task<CuentaBancaria?> obtenerCuentaBancariaPorId(int id);
    }
}