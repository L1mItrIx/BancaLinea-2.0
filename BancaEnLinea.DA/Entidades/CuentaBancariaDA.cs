using BancaEnLinea.BC.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancaEnLinea.DA.Entidades
{
    [Table("CuentaBancaria")]
    public class CuentaBancariaDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public long NumeroTarjeta { get; set; }

        [Required]
        public TipoCuenta Tipo { get; set; }

        [Required]
        public Moneda Moneda { get; set; }

        [Required]
        public long Saldo { get; set; }

        [Required]
        public EstadoCB Estado { get; set; }

        [Required]
        [ForeignKey("Cuenta")]
        public int IdCuenta { get; set; }

        public virtual CuentaDA Cuenta { get; set; }
    }
}