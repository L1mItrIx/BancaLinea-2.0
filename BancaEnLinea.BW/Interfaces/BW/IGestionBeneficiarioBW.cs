using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionBeneficiarioBW
    {
        Task<bool> registrarBeneficiario(Beneficiarios beneficiario);
   Task<List<Beneficiarios>> obtenerBeneficiariosPorCliente(int idCuenta);
Task<List<Beneficiarios>> obtenerTodosLosBeneficiarios();
 Task<Beneficiarios?> obtenerBeneficiarioPorId(int idBeneficiario);
 Task<bool> actualizarBeneficiario(Beneficiarios beneficiario, int idBeneficiario);
        Task<bool> eliminarBeneficiario(int idBeneficiario);
        Task<List<Beneficiarios>> obtenerBeneficiariosPendientes();
        Task<(bool exito, string mensaje)> confirmarBeneficiario(int idBeneficiario);
    Task<(bool exito, string mensaje)> rechazarBeneficiario(int idBeneficiario);
    }
}
