using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.Modelos;
namespace BancaEnLinea.BC.ReglasDeNegocio
{
    public static class ReglasDeCuentaBancaria
    {
        public static bool laCuentaBancariaEsValida(CuentaBancaria cuentaBancaria)
        {
            string numeroTarjetastr = cuentaBancaria.NumeroTarjeta.ToString();
            return cuentaBancaria != null &&
                numeroTarjetastr.Length == 12 &&
                cuentaBancaria.NumeroTarjeta > 0 &&
                Enum.IsDefined(typeof(TipoCuenta), cuentaBancaria.Tipo) &&
                Enum.IsDefined(typeof(Moneda), cuentaBancaria.Moneda) &&
                Enum.IsDefined(typeof(EstadoCB), cuentaBancaria.Estado) &&
                cuentaBancaria.Saldo > 0;
        }
        public static bool puedeCrearNuevaCuenta(IEnumerable<CuentaBancaria> cuentasExistentes, CuentaBancaria nuevaCuenta)
        {
            if (nuevaCuenta == null || cuentasExistentes == null)
                return false;

            int cuentasMismoTipoYMoneda = cuentasExistentes.Count(contador =>
                contador.Tipo == nuevaCuenta.Tipo &&
                contador.Moneda == nuevaCuenta.Moneda
            );

            return cuentasMismoTipoYMoneda < 3;
        }
    }
}
