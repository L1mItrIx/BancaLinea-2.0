using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    public class Beneficiarios
    {
        public int IdBeneficiario { get; set; }
        public int IdCuenta { get; set; }
        public string Alias { get; set; }
        public string Banco { get; set; }
        public Moneda Moneda { get; set; }
        public long NumeroCuentaDestino { get; set; }
        public string Pais { get; set; }
        public EstadoP Estado { get; set; }

        // Propiedad de navegación opcional para incluir datos del cliente
        public Cuenta? Cuenta { get; set; }
    }
}
