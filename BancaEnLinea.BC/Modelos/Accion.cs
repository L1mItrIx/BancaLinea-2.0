namespace BancaEnLinea.BC.Modelos
{
    /// <summary>
    /// Registro simple de acciones en el sistema
    /// </summary>
    public class Accion
    {
      public int Id { get; set; }
  public DateTime Fecha { get; set; }
      public string Descripcion { get; set; }  // Ej: "Se creó una cuenta", "Se realizó una transferencia"
      public int? IdUsuario { get; set; }  // Opcional: ID del usuario que hizo la acción
      public string? NombreUsuario { get; set; }  // Opcional: Nombre del usuario
    }
}
