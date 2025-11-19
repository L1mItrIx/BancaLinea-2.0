using BancaEnLinea.BC.Enums;
namespace BancaEnLinea.BC.Modelos
{
    public class CuentaBancaria
    {
        public int Id { get; set; }
        public long NumeroTarjeta { get; set; }
        public TipoCuenta Tipo {  get; set; }
        public Moneda Moneda { get; set; }
        public long Saldo { get; set; }
        public EstadoCB Estado { get; set; }
        public int IdCuenta { get; set; }
        
        // Propiedad de navegación para incluir datos del cliente
        public Cuenta? Cuenta { get; set; }
    }
}
