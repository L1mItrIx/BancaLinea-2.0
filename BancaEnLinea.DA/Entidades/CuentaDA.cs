using BancaEnLinea.BC.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
  [Table("Cuenta")]
    public class CuentaDA
    {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }
      [Required]
      public long Telefono { get; set; }
      [Required]
      public string Nombre { get; set; }
      [Required]
      public string PrimerApellido { get; set; }
      [Required]
      public string SegundoApellido { get; set; }
      public string Correo { get; set; }
      [Required]
      public string Contrasena { get; set; }
      [Required]
      public RolCuenta Rol { get; set; }
    }
}
