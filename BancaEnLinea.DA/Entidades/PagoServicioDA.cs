using BancaEnLinea.BC.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("PagoServicio")]
    public class PagoServicioDA
    {
   [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int IdPago { get; set; }

        [Required]
        [ForeignKey("ContratoServicio")]
  public int IdContratoServicio { get; set; }

        [Required]
      [ForeignKey("CuentaBancariaOrigen")]
   public int IdCuentaBancariaOrigen { get; set; }

        [Required]
   public long Monto { get; set; }

        [Required]
 public long Comision { get; set; }

   [Required]
        public long MontoTotal { get; set; }

        [Required]
     public DateTime FechaCreacion { get; set; }

    [Required]
   public DateTime FechaEjecucion { get; set; }

        [Required]
        public EstadoTra Estado { get; set; }

        // Navegaciones
public virtual ContratoServicioDA ContratoServicio { get; set; }
    public virtual CuentaBancariaDA CuentaBancariaOrigen { get; set; }
    }
}
