using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("Servicio")]
    public class ServicioDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdServicio { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(500)]
     public string? Descripcion { get; set; }

        [Required]
        public long Contrato { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
      public decimal Costo { get; set; }
    }
}
