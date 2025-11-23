using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
    /// DTO para crear una transferencia (lo que envía el frontend)
    /// SOLO los datos esenciales que el usuario proporciona
    /// </summary>
    public class TransferenciaRequest
    {
        public int IdCuentaBancariaOrigen { get; set; }
        public long NumeroCuentaDestino { get; set; }
        public long Monto { get; set; }
        
        /// <summary>
        /// Si es NULL o DateTime.Now.Date = Transferencia INMEDIATA
        /// Si tiene fecha futura = Transferencia PROGRAMADA (máx 90 días)
        /// </summary>
        public DateTime? FechaEjecucion { get; set; }
        
        public string? Descripcion { get; set; }
    }
}
