using BancaEnLinea.BC.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("Beneficiario")]
    public class BeneficiarioDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBeneficiario { get; set; }

        [Required]
        [ForeignKey("Cuenta")]
        public int IdCuenta { get; set; }

        [Required]
        [MaxLength(30)]
        public string Alias { get; set; }

        [Required]
        [MaxLength(100)]
        public string Banco { get; set; }

        [Required]
        public Moneda Moneda { get; set; }

        [Required]
        public long NumeroCuentaDestino { get; set; }

        [Required]
        [MaxLength(50)]
        public string Pais { get; set; }

        [Required]
        public EstadoP Estado { get; set; }

        // Navegación hacia Cuenta (Cliente)
        public virtual CuentaDA Cuenta { get; set; }
    }
}
