using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
    /// DTO para la respuesta de transferencias con información de monedas
  /// </summary>
    public class TransferenciaResponse
    {
        // Datos básicos de la transferencia
        public int Referencia { get; set; }
        public long NumeroCuentaDestino { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEjecucion { get; set; }
      public EstadoTra Estado { get; set; }
public string? Descripcion { get; set; }

        // Información de la cuenta origen
  public long NumeroCuentaOrigen { get; set; }
     public Moneda MonedaOrigen { get; set; }
        public string MonedaOrigenTexto { get; set; }  // "CRC" o "USD"

        // Montos en moneda de origen
        public long MontoEnviado { get; set; }
        public long ComisionEnviada { get; set; }
        public long TotalDebitado { get; set; }

        // Información de conversión (si aplica)
public bool RequiereConversion { get; set; }
        public Moneda? MonedaDestino { get; set; }
        public string? MonedaDestinoTexto { get; set; }  // "CRC" o "USD"
        public long? MontoRecibido { get; set; }  // Monto en moneda destino después de conversión
        public decimal? TipoCambioAplicado { get; set; }  // Tipo de cambio usado

 // Información adicional
        public string EstadoTexto { get; set; }  // "Exitosa", "Pendiente", etc.
}

    /// <summary>
    /// DTO para transferencias recibidas con información de conversión
    /// </summary>
    public class TransferenciaRecibidaResponse
    {
  public int Referencia { get; set; }
        public long NumeroCuentaOrigen { get; set; }
 public DateTime FechaCreacion { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public string? Descripcion { get; set; }

 // Información de monedas
        public Moneda MonedaOrigen { get; set; }
  public string MonedaOrigenTexto { get; set; }
        public Moneda MonedaTuCuenta { get; set; }
        public string MonedaTuCuentaTexto { get; set; }

        // Montos
        public long MontoEnviadoPorRemitente { get; set; }  // En moneda origen
 public long MontoRecibidoEnTuCuenta { get; set; }  // En tu moneda (después de conversión)
        
        // Conversión
        public bool HuboConversion { get; set; }
      public decimal? TipoCambioAplicado { get; set; }
    }
}
