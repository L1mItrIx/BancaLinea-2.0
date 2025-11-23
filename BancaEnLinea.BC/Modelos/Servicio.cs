namespace BancaEnLinea.BC.Modelos
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } 
        public string Descripcion { get; set; }
        public long Contrato { get; set; }
        public decimal Costo { get; set; }
    }
}
