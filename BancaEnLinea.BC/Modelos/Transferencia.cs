using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    public class Transferencia
    {
        public int Referencia { get; set; }
        public string IdempotencyKey { get; set; }
        public int IdCuentaBancariaOrigen { get; set; }
        public long NumeroCuentaDestino { get; set; }
        public long Monto { get; set; }
        public long Comision { get; set; }
        public long MontoTotal { get; set; }
        public long SaldoAnterior { get; set; }
        public long SaldoPosterior { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public EstadoTra Estado { get; set; }
        public string? Descripcion { get; set; }
        
        // Propiedades de navegación
        public CuentaBancaria? CuentaBancariaOrigen { get; set; }
        public Cuenta? Aprobador { get; set; }
    }
}
