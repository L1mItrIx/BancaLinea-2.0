using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BW.Interfaces.BW
{
    public interface IGestionContratoServicioBW
    {
   Task<(bool exito, string mensaje)> agregarContrato(ContratoServicio contrato);
        Task<List<ContratoServicio>> obtenerTodosLosContratos();
        Task<List<ContratoServicio>> obtenerContratosPorServicio(int idServicio);
        Task<(bool exito, string mensaje)> eliminarContrato(int idContratoServicio);
        Task<List<ContratoServicio>> obtenerTodosLosContratosPendientes();
    }
}
