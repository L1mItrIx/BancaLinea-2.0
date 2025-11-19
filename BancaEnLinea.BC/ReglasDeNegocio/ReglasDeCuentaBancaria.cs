using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.Modelos;
namespace BancaEnLinea.BC.ReglasDeNegocio
{
    public static class ReglasDeCuentaBancaria
    {
        public static bool laCuentaBancariaEsValida(CuentaBancaria cuentaBancaria)
   {
         if (cuentaBancaria == null)
     return false;

       string numeroTarjetastr = cuentaBancaria.NumeroTarjeta.ToString();
       
   // Validar que el número de tarjeta tenga 12 dígitos
            bool numeroTarjetaValido = numeroTarjetastr.Length == 12 && cuentaBancaria.NumeroTarjeta > 0;
 
            // Validar enums
 bool enumsValidos = Enum.IsDefined(typeof(TipoCuenta), cuentaBancaria.Tipo) &&
           Enum.IsDefined(typeof(Moneda), cuentaBancaria.Moneda) &&
                 Enum.IsDefined(typeof(EstadoCB), cuentaBancaria.Estado);
   
            // Validar saldo (puede ser 0 o mayor)
      bool saldoValido = cuentaBancaria.Saldo >= 0;

       return numeroTarjetaValido && enumsValidos && saldoValido;
        }

        public static bool puedeCrearNuevaCuenta(IEnumerable<CuentaBancaria> cuentasExistentes, CuentaBancaria nuevaCuenta)
        {
        if (nuevaCuenta == null || cuentasExistentes == null)
                return false;

          // Contar solo las cuentas activas y bloqueadas (no cerradas) del mismo tipo y moneda
   int cuentasMismoTipoYMoneda = cuentasExistentes.Count(contador =>
          contador.Tipo == nuevaCuenta.Tipo &&
       contador.Moneda == nuevaCuenta.Moneda &&
  contador.Estado != EstadoCB.Cerrada
      );

  return cuentasMismoTipoYMoneda < 3;
        }

        /// <summary>
        /// Valida que solo los usuarios con rol Cliente puedan tener cuentas bancarias.
        /// Administradores (Rol = 0) y Gestores (Rol = 1) NO pueden tener cuentas bancarias.
        /// </summary>
        public static bool puedeTenerCuentaBancaria(Cuenta cuenta)
 {
            if (cuenta == null)
     return false;

     // Solo los clientes (Rol = 2) pueden tener cuentas bancarias
       return cuenta.Rol == RolCuenta.Cliente;
  }

        /// <summary>
        /// Determina el estado de la cuenta bancaria basado en el saldo.
/// Si el saldo es 0, la cuenta debe estar cerrada.
        /// </summary>
   public static EstadoCB determinarEstadoPorSaldo(long saldo, EstadoCB estadoActual)
        {
    if (saldo == 0)
            {
           return EstadoCB.Cerrada;
}
            
     // Si el saldo es mayor a 0 y la cuenta estaba cerrada, la reactivamos
  if (saldo > 0 && estadoActual == EstadoCB.Cerrada)
       {
                return EstadoCB.Activa;
            }

            // Mantener el estado actual si no es cerrada
      return estadoActual;
      }
    }
}
