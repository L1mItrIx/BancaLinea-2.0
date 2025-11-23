using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
    /// DTO para realizar un pago de servicio - SOLO lo necesario del cliente
    /// </summary>
    public class PagoServicioRequest
    {
        public long NumeroContrato { get; set; }  // Número de contrato a pagar
        public long Monto { get; set; }
        public int IdCuentaBancariaOrigen { get; set; }
        public DateTime? FechaEjecucion { get; set; }  // null = INMEDIATO
    }

    /// <summary>
    /// Modelo completo de pago de servicio
    /// </summary>
    public class PagoServicio
    {
        public int IdPago { get; set; }
        public int IdContratoServicio { get; set; }
        public int IdCuentaBancariaOrigen { get; set; }
        public long Monto { get; set; }
        public long Comision { get; set; }
        public long MontoTotal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public EstadoTra Estado { get; set; }

        // Navegaciones
        public ContratoServicio? ContratoServicio { get; set; }
        public CuentaBancaria? CuentaBancariaOrigen { get; set; }
    }
}
