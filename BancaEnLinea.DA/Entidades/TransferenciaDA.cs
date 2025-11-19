using BancaEnLinea.BC.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("Transferencia")]
    public class TransferenciaDA
{
        [Key]
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Referencia { get; set; }

        [Required]
        [MaxLength(100)]
        public string IdempotencyKey { get; set; }

        [Required]
        [ForeignKey("CuentaBancariaOrigen")]
        public int IdCuentaBancariaOrigen { get; set; }

        [Required]
        public long NumeroCuentaDestino { get; set; }

        [Required]
        public long Monto { get; set; }

        [Required]
 public long Comision { get; set; }

        [Required]
        public long MontoTotal { get; set; }

        [Required]
     public long SaldoAnterior { get; set; }

        [Required]
        public long SaldoPosterior { get; set; }

        [Required]
     public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaEjecucion { get; set; }

        [Required]
    public EstadoTra Estado { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [ForeignKey("Aprobador")]
     public int? IdAprobador { get; set; }

        // Navegaciones
        public virtual CuentaBancariaDA CuentaBancariaOrigen { get; set; }
        public virtual CuentaDA? Aprobador { get; set; }
    }
}
