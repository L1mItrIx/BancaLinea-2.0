using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
  public class GestionTransferenciaBW : IGestionTransferenciaBW
    {
 private readonly IGestionTransferenciaDA gestionTransferenciaDA;
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;
 private readonly IGestionCuentaDA gestionCuentaDA;

 public GestionTransferenciaBW(
    IGestionTransferenciaDA gestionTransferenciaDA,
     IGestionCuentaBancariaDA gestionCuentaBancariaDA,
       IGestionCuentaDA gestionCuentaDA)
 {
    this.gestionTransferenciaDA = gestionTransferenciaDA;
     this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
   this.gestionCuentaDA = gestionCuentaDA;
      }

        public async Task<(bool exito, string mensaje, int? referencia)> registrarTransferencia(Transferencia transferencia)
  {
    // 1. Validar datos básicos
      if (!ReglasDeTransferencia.laTransferenciaEsValida(transferencia))
     return (false, "Datos de transferencia inválidos", null);

    // 2. Validar fecha de ejecución (no más de 90 días)
  if (!ReglasDeTransferencia.laFechaEjecucionEsValida(transferencia.FechaEjecucion))
     return (false, $"La fecha de ejecución no puede superar {ReglasDeTransferencia.DIAS_MAXIMOS_PROGRAMACION} días", null);

      // 3. Obtener y validar cuenta bancaria origen
     var cuentaBancariaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(transferencia.IdCuentaBancariaOrigen);
    if (cuentaBancariaOrigen == null)
       return (false, "Cuenta bancaria de origen no encontrada", null);

  // 4. Validar que la cuenta bancaria esté activa
if (!ReglasDeTransferencia.laCuentaBancariaEstaActiva(cuentaBancariaOrigen))
    return (false, "La cuenta bancaria de origen no está activa", null);

// 5. Obtener y validar cuenta del cliente
 var cliente = await gestionCuentaDA.obtenerCuentaPorId(cuentaBancariaOrigen.IdCuenta);
     if (!ReglasDeTransferencia.elClienteEstaActivo(cliente))
  return (false, "El cliente no está activo o no tiene permisos", null);

 // 6. Calcular montos
     transferencia.Comision = ReglasDeTransferencia.COMISION_FIJA;
  transferencia.MontoTotal = ReglasDeTransferencia.calcularMontoTotal(transferencia.Monto);

 // 7. Validar saldo suficiente
if (!ReglasDeTransferencia.elSaldoEsSuficiente(cuentaBancariaOrigen.Saldo, transferencia.MontoTotal))
   return (false, "Saldo insuficiente para realizar la transferencia", null);

       // 8. Validar límite diario
      var transferenciasDelDia = await gestionTransferenciaDA.obtenerTransferenciasDelDia(transferencia.IdCuentaBancariaOrigen);
if (!ReglasDeTransferencia.noSuperaLimiteDiario(transferenciasDelDia, transferencia.MontoTotal))
   return (false, "Se ha excedido el límite diario de transferencias", null);

 // 9. Calcular saldos
         transferencia.SaldoAnterior = cuentaBancariaOrigen.Saldo;
   transferencia.SaldoPosterior = ReglasDeTransferencia.calcularSaldoPosterior(cuentaBancariaOrigen.Saldo, transferencia.MontoTotal);

   // 10. Establecer fechas
     transferencia.FechaCreacion = DateTime.Now;

      // 11. Determinar estado inicial
    transferencia.Estado = ReglasDeTransferencia.determinarEstadoInicial(transferencia.Monto, transferencia.FechaEjecucion);

 // 12. Registrar la transferencia
      int referencia = await gestionTransferenciaDA.registrarTransferencia(transferencia);
 if (referencia <= 0)
     return (false, "Error al registrar la transferencia", null);

    // 13. Si no requiere aprobación y es para hoy, ejecutar inmediatamente
       if (transferencia.Estado == EstadoTra.Pendiente && 
!ReglasDeTransferencia.requiereAprobacion(transferencia.Monto) && 
    transferencia.FechaEjecucion.Date == DateTime.Now.Date)
   {
 var resultadoEjecucion = await ejecutarTransferencia(referencia);
if (!resultadoEjecucion.exito)
        return (false, resultadoEjecucion.mensaje, referencia);
     }

            // 14. Retornar mensaje según el estado
  string mensaje = transferencia.Estado switch
  {
 EstadoTra.Pendiente => $"Transferencia pendiente de aprobación (supera ?{ReglasDeTransferencia.UMBRAL_APROBACION:N0})",
    EstadoTra.Programada => $"Transferencia programada para {transferencia.FechaEjecucion:dd/MM/yyyy}",
  EstadoTra.Exitosa => "Transferencia realizada exitosamente",
   _ => "Transferencia registrada"
 };

  return (true, mensaje, referencia);
}

 public Task<List<Transferencia>> obtenerTransferenciasPorCuentaBancaria(int idCuentaBancaria)
 {
     return gestionTransferenciaDA.obtenerTransferenciasPorCuentaBancaria(idCuentaBancaria);
}

     public Task<List<Transferencia>> obtenerTodasLasTransferencias()
        {
  return gestionTransferenciaDA.obtenerTodasLasTransferencias();
 }

        public Task<Transferencia?> obtenerTransferenciaPorReferencia(int referencia)
 {
 return gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
        }

        public Task<List<Transferencia>> obtenerTransferenciasPendientes()
{
       return gestionTransferenciaDA.obtenerTransferenciasPendientes();
   }

      public async Task<(bool exito, string mensaje)> cancelarTransferencia(int referencia, int idCliente)
   {
      var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
  if (transferencia == null)
   return (false, "Transferencia no encontrada");

    // Validar que la transferencia pertenezca al cliente
   var cuentaBancariaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(transferencia.IdCuentaBancariaOrigen);
   if (cuentaBancariaOrigen?.IdCuenta != idCliente)
   return (false, "No tiene permisos para cancelar esta transferencia");

    // Validar que se pueda cancelar
       if (!ReglasDeTransferencia.puedeCancelarse(transferencia))
return (false, $"Solo se pueden cancelar transferencias con al menos {ReglasDeTransferencia.HORAS_MINIMAS_CANCELACION} horas de anticipación");

    bool resultado = await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Cancelada);
      return resultado ? (true, "Transferencia cancelada exitosamente") : (false, "Error al cancelar la transferencia");
        }

        public async Task<(bool exito, string mensaje)> aprobarTransferencia(int referencia, int idAprobador)
 {
     var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
  if (transferencia == null)
      return (false, "Transferencia no encontrada");

   // Validar que esté pendiente
if (transferencia.Estado != EstadoTra.Pendiente)
    return (false, "Solo se pueden aprobar transferencias pendientes");

// Validar que el aprobador sea Admin o Gestor
        var aprobador = await gestionCuentaDA.obtenerCuentaPorId(idAprobador);
  if (!ReglasDeTransferencia.elAprobadorEsValido(aprobador))
            return (false, "No tiene permisos para aprobar transferencias");

   // Ejecutar la transferencia
  var resultadoEjecucion = await ejecutarTransferencia(referencia);
      if (!resultadoEjecucion.exito)
     return (false, resultadoEjecucion.mensaje);

            // Registrar aprobación
      bool resultado = await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Exitosa, idAprobador);
     return resultado ? (true, "Transferencia aprobada y ejecutada exitosamente") : (false, "Error al aprobar la transferencia");
 }

     public async Task<(bool exito, string mensaje)> rechazarTransferencia(int referencia, int idAprobador, string motivo)
   {
 var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
      if (transferencia == null)
   return (false, "Transferencia no encontrada");

  // Validar que esté pendiente
   if (transferencia.Estado != EstadoTra.Pendiente)
   return (false, "Solo se pueden rechazar transferencias pendientes");

            // Validar que el aprobador sea Admin o Gestor
 var aprobador = await gestionCuentaDA.obtenerCuentaPorId(idAprobador);
            if (!ReglasDeTransferencia.elAprobadorEsValido(aprobador))
     return (false, "No tiene permisos para rechazar transferencias");

     bool resultado = await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Rechazada, idAprobador, motivo);
 return resultado ? (true, "Transferencia rechazada") : (false, "Error al rechazar la transferencia");
        }

    // Método privado para ejecutar una transferencia
     private async Task<(bool exito, string mensaje)> ejecutarTransferencia(int referencia)
 {
   var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
     if (transferencia == null)
       return (false, "Transferencia no encontrada");

  var cuentaBancariaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(transferencia.IdCuentaBancariaOrigen);
  if (cuentaBancariaOrigen == null)
return (false, "Cuenta bancaria no encontrada");

       // Actualizar saldo de la cuenta origen (debitar)
 cuentaBancariaOrigen.Saldo = transferencia.SaldoPosterior;

// Actualizar estado según saldo
cuentaBancariaOrigen.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancariaOrigen.Saldo, cuentaBancariaOrigen.Estado);

    bool saldoActualizado = await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancariaOrigen, cuentaBancariaOrigen.Id);
  if (!saldoActualizado)
 return (false, "Error al actualizar el saldo");

       // Actualizar estado de la transferencia
    await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Exitosa);

  return (true, "Transferencia ejecutada exitosamente");
}
    }
}
