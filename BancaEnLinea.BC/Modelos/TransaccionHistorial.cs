using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
    /// DTO para el historial de transacciones unificado
    /// </summary>
    public class TransaccionHistorial
    {
  public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
 public DateTime FechaEjecucion { get; set; }
        public string Tipo { get; set; } // "Transferencia" o "Pago de Servicio"
        public string TipoIcono { get; set; } // "transfer" o "payment"
   
        // Información del cliente
        public int IdCliente { get; set; }
    public string NombreCliente { get; set; }
        
    // Información de la cuenta
        public long NumeroCuenta { get; set; }
        public Moneda MonedaCuenta { get; set; }
        public string MonedaCuentaTexto { get; set; }
        public string SimboloMoneda { get; set; }
        
        // Montos
   public long Monto { get; set; }
     public long Comision { get; set; }
        public long MontoTotal { get; set; }
        
        // Estado
        public EstadoTra Estado { get; set; }
        public string EstadoTexto { get; set; }
        
   // Descripción/Detalles
 public string Descripcion { get; set; }
 public string Destino { get; set; } // Número de cuenta destino o servicio
        
        // Referencia única
 public string Referencia { get; set; }
 }
}
