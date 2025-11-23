namespace BancaEnLinea.BC.Modelos
{
    public class ContratoServicio
    {
        public int IdContratoServicio { get; set; }
        public int IdServicio { get; set; }
        public long NumeroContrato { get; set; }
        
    // Navegación
        public Servicio? Servicio { get; set; }
    }
}
