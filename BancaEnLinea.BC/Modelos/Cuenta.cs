using BancaEnLinea.BC.Enums;
namespace BancaEnLinea.BC.Modelos
{
    public class Cuenta
    {
      public int Id { get; set; }
      public long Telefono { get; set; }
      public string Nombre { get; set; }
      public string PrimerApellido { get; set; }
      public string SegundoApellido { get; set; }
      public string Correo { get; set; }
      public string Contrasena { get; set; }
      public RolCuenta Rol {  get; set; }
    }
}
