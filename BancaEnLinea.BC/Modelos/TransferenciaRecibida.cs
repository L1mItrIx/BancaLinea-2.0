namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
    /// DTO simplificado para mostrar transferencias recibidas
    /// </summary>
    public class TransferenciaRecibida
    {
        public int Referencia { get; set; }
        public long Monto { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public string Remitente { get; set; }  // Nombre completo del que envió
  public long CuentaOrigen { get; set; }  // Número de cuenta del remitente
        public string Descripcion { get; set; }
    }
}
