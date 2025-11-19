using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BC.ReglasDeNegocio
{
    public static class ReglasDeTransferencia
    {
        // Constantes de negocio
        public const long UMBRAL_APROBACION = 100000; // 100,000 colones
        public const long LIMITE_DIARIO_DEFAULT = 500000; // 500,000 colones por día
   public const long COMISION_FIJA = 500; // 500 colones de comisión
  public const int DIAS_MAXIMOS_PROGRAMACION = 90;
      public const int HORAS_MINIMAS_CANCELACION = 24;

        /// <summary>
        /// Valida que la transferencia tenga todos los datos requeridos
        /// </summary>
        public static bool laTransferenciaEsValida(Transferencia transferencia)
   {
   if (transferencia == null)
   return false;

      // Validar IdempotencyKey (obligatorio, entre 10 y 100 caracteres)
            bool idempotencyValido = !string.IsNullOrWhiteSpace(transferencia.IdempotencyKey) &&
    transferencia.IdempotencyKey.Length >= 10 &&
 transferencia.IdempotencyKey.Length <= 100;

    // Validar cuenta bancaria origen
  bool cuentaOrigenValida = transferencia.IdCuentaBancariaOrigen > 0;

       // Validar número de cuenta destino (12-20 dígitos)
    string numeroCuentaStr = transferencia.NumeroCuentaDestino.ToString();
          bool cuentaDestinoValida = transferencia.NumeroCuentaDestino > 0 &&
    numeroCuentaStr.Length >= 12 &&
   numeroCuentaStr.Length <= 20;

// Validar monto (debe ser mayor a 0)
          bool montoValido = transferencia.Monto > 0;

            // Validar fecha de ejecución (no puede ser en el pasado)
 bool fechaValida = transferencia.FechaEjecucion >= DateTime.Now.Date;

    // Validar estado
    bool estadoValido = Enum.IsDefined(typeof(EstadoTra), transferencia.Estado);

            return idempotencyValido && cuentaOrigenValida && cuentaDestinoValida &&
        montoValido && fechaValida && estadoValido;
        }

        /// <summary>
      /// Valida que la cuenta bancaria de origen esté activa
   /// </summary>
   public static bool laCuentaBancariaEstaActiva(CuentaBancaria cuentaBancaria)
   {
          return cuentaBancaria != null && cuentaBancaria.Estado == EstadoCB.Activa;
     }

     /// <summary>
        /// Valida que el cliente (cuenta) esté activo y sea cliente
        /// </summary>
public static bool elClienteEstaActivo(Cuenta cuenta)
   {
       return cuenta != null && cuenta.Rol == RolCuenta.Cliente;
        }

        /// <summary>
        /// Valida que el saldo sea suficiente para la transferencia
        /// </summary>
        public static bool elSaldoEsSuficiente(long saldoActual, long montoTotal)
   {
      return saldoActual >= montoTotal;
 }

        /// <summary>
      /// Calcula el monto total a debitar (monto + comisión)
        /// </summary>
    public static long calcularMontoTotal(long monto)
        {
      return monto + COMISION_FIJA;
        }

      /// <summary>
        /// Calcula el saldo posterior a la transferencia
     /// </summary>
        public static long calcularSaldoPosterior(long saldoActual, long montoTotal)
   {
      return saldoActual - montoTotal;
    }

    /// <summary>
        /// Valida que la transferencia no supere el límite diario
   /// </summary>
        public static bool noSuperaLimiteDiario(IEnumerable<Transferencia> transferenciasDelDia, long montoNuevo, long limiteDiario = LIMITE_DIARIO_DEFAULT)
        {
  // Sumar solo transferencias exitosas o pendientes de hoy
   long totalDelDia = transferenciasDelDia
    .Where(t => t.FechaCreacion.Date == DateTime.Now.Date &&
      (t.Estado == EstadoTra.Exitosa || t.Estado == EstadoTra.Pendiente || t.Estado == EstadoTra.Programada))
                .Sum(t => t.MontoTotal);

 return (totalDelDia + montoNuevo) <= limiteDiario;
        }

  /// <summary>
        /// Determina si la transferencia requiere aprobación por superar el umbral
/// </summary>
        public static bool requiereAprobacion(long monto)
        {
    return monto >= UMBRAL_APROBACION;
        }

        /// <summary>
     /// Valida que la fecha de ejecución no supere los 90 días
      /// </summary>
        public static bool laFechaEjecucionEsValida(DateTime fechaEjecucion)
 {
     DateTime fechaMaxima = DateTime.Now.AddDays(DIAS_MAXIMOS_PROGRAMACION);
            return fechaEjecucion >= DateTime.Now.Date && fechaEjecucion <= fechaMaxima;
        }

   /// <summary>
        /// Valida si una transferencia puede ser cancelada (24 horas antes de ejecución)
        /// </summary>
     public static bool puedeCancelarse(Transferencia transferencia)
        {
  if (transferencia == null)
          return false;

       // Solo se pueden cancelar transferencias Pendientes o Programadas
          if (transferencia.Estado != EstadoTra.Pendiente && transferencia.Estado != EstadoTra.Programada)
     return false;

            // Debe faltar al menos 24 horas para la ejecución
            TimeSpan tiempoRestante = transferencia.FechaEjecucion - DateTime.Now;
     return tiempoRestante.TotalHours >= HORAS_MINIMAS_CANCELACION;
    }

        /// <summary>
 /// Valida que el IdempotencyKey sea único
    /// </summary>
        public static bool elIdempotencyKeyEsUnico(IEnumerable<Transferencia> transferencias, string idempotencyKey)
 {
       return !transferencias.Any(t => t.IdempotencyKey == idempotencyKey);
     }

        /// <summary>
  /// Determina el estado inicial de la transferencia
/// </summary>
        public static EstadoTra determinarEstadoInicial(long monto, DateTime fechaEjecucion)
   {
        // Si requiere aprobación
   if (requiereAprobacion(monto))
           return EstadoTra.Pendiente;

     // Si es para ejecutar hoy
      if (fechaEjecucion.Date == DateTime.Now.Date)
          return EstadoTra.Pendiente;

       // Si es programada a futuro
   return EstadoTra.Programada;
  }

        /// <summary>
        /// Valida que un aprobador sea Admin o Gestor
        /// </summary>
     public static bool elAprobadorEsValido(Cuenta aprobador)
        {
     return aprobador != null &&
   (aprobador.Rol == RolCuenta.Administrador || aprobador.Rol == RolCuenta.Gestor);
  }

    /// <summary>
/// Valida que el ID de la transferencia sea válido
  /// </summary>
        public static bool elIdEsValido(int id)
        {
      return id > 0;
        }
    }
}
