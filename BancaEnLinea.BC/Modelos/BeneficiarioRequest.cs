using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
 /// DTO para registrar un beneficiario - SOLO lo necesario del cliente
    /// </summary>
    public class BeneficiarioRequest
    {
        public int IdCuenta { get; set; }
        public string Alias { get; set; }
        public string Banco { get; set; }
        public Moneda Moneda { get; set; }
        public long NumeroCuentaDestino { get; set; }
        public string Pais { get; set; }
    }
}
