using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.DA
{
    public interface IGestionContratoServicioDA
 {
     Task<bool> agregarContrato(ContratoServicio contrato);
        Task<List<ContratoServicio>> obtenerTodosLosContratos();
   Task<List<ContratoServicio>> obtenerContratosPorServicio(int idServicio);
        Task<ContratoServicio?> obtenerContratoPorNumero(long numeroContrato);
    Task<bool> eliminarContrato(int idContratoServicio);
        Task<List<ContratoServicio>> obtenerTodosLosContratosPendientes();
    }
}
