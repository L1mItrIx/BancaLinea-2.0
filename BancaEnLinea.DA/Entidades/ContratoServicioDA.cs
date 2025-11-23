using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("ContratoServicio")]
 public class ContratoServicioDA
    {
 [Key]
 [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int IdContratoServicio { get; set; }

        [Required]
 [ForeignKey("Servicio")]
        public int IdServicio { get; set; }

        [Required]
        public long NumeroContrato { get; set; }

      // Navegación
     public virtual ServicioDA Servicio { get; set; }
 }
}
