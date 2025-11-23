using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
  public class GestionContratoServicioBW : IGestionContratoServicioBW
    {
   private readonly IGestionContratoServicioDA gestionContratoServicioDA;

  public GestionContratoServicioBW(IGestionContratoServicioDA gestionContratoServicioDA)
        {
     this.gestionContratoServicioDA = gestionContratoServicioDA;
 }

  public async Task<(bool exito, string mensaje)> agregarContrato(ContratoServicio contrato)
  {
      // Validar número de contrato (8-12 dígitos)
 string numeroStr = contrato.NumeroContrato.ToString();
    if (numeroStr.Length < 8 || numeroStr.Length > 12)
    return (false, "El número de contrato debe tener entre 8 y 12 dígitos");

// Verificar que no exista el mismo número
  var contratoExistente = await gestionContratoServicioDA.obtenerContratoPorNumero(contrato.NumeroContrato);
     if (contratoExistente != null)
   return (false, "Este número de contrato ya existe");

      bool resultado = await gestionContratoServicioDA.agregarContrato(contrato);
  return resultado
       ? (true, "Contrato agregado exitosamente")
 : (false, "Error al agregar el contrato");
        }

     public Task<List<ContratoServicio>> obtenerTodosLosContratos()
{
  return gestionContratoServicioDA.obtenerTodosLosContratos();
  }

    public Task<List<ContratoServicio>> obtenerContratosPorServicio(int idServicio)
   {
    return gestionContratoServicioDA.obtenerContratosPorServicio(idServicio);
 }

     public async Task<(bool exito, string mensaje)> eliminarContrato(int idContratoServicio)
        {
 bool resultado = await gestionContratoServicioDA.eliminarContrato(idContratoServicio);
     return resultado
       ? (true, "Contrato eliminado exitosamente")
: (false, "Error al eliminar el contrato");
        }

     public Task<List<ContratoServicio>> obtenerTodosLosContratosPendientes()
        {
   return gestionContratoServicioDA.obtenerTodosLosContratosPendientes();
 }
    }
}
