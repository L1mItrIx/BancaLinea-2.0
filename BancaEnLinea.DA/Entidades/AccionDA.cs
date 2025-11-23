using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("Accion")]
    public class AccionDA
    {
        [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

 [Required]
        public DateTime Fecha { get; set; }

      [Required]
  [MaxLength(500)]
        public string Descripcion { get; set; }

     public int? IdUsuario { get; set; }

        [MaxLength(200)]
        public string? NombreUsuario { get; set; }
    }
}
