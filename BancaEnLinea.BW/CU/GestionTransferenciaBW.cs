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

        public async Task<(bool exito, string mensaje, int? referencia)> registrarTransferencia(TransferenciaRequest request)
        {
            var transferencia = construirTransferenciaDesdeRequest(request);

      var validacion = await validarTransferencia(transferencia);
      if (!validacion.esValida)
     return (false, validacion.mensaje, null);

        var cuentaBancariaOrigen = validacion.cuentaBancariaOrigen;
   configurarMontosYComisiones(transferencia, cuentaBancariaOrigen);

   var validacionMontos = await validarMontosYLimites(transferencia, cuentaBancariaOrigen);
if (!validacionMontos.esValido)
                return (false, validacionMontos.mensaje, null);

          configurarSaldosYEstado(transferencia, cuentaBancariaOrigen);

          int referenciaId = await gestionTransferenciaDA.registrarTransferencia(transferencia);
        if (referenciaId <= 0)
          return (false, "Error al registrar la transferencia", null);

            var resultadoEjecucion = await ejecutarSiCorresponde(transferencia, referenciaId);
         if (!resultadoEjecucion.exito)
     return (false, resultadoEjecucion.mensaje, referenciaId);

     string mensaje = generarMensajeConfirmacion(transferencia);
            return (true, mensaje, referenciaId);
      }

        private Transferencia construirTransferenciaDesdeRequest(TransferenciaRequest request)
        {
            return new Transferencia
    {
                IdempotencyKey = Guid.NewGuid().ToString(),
 IdCuentaBancariaOrigen = request.IdCuentaBancariaOrigen,
        NumeroCuentaDestino = request.NumeroCuentaDestino,
           Monto = request.Monto,
  FechaEjecucion = request.FechaEjecucion.HasValue && request.FechaEjecucion.Value.Date > DateTime.Now.Date
          ? request.FechaEjecucion.Value.Date
 : DateTime.Now.Date,
    Descripcion = request.Descripcion
         };
        }

        private async Task<(bool esValida, string mensaje, CuentaBancaria cuentaBancariaOrigen)> validarTransferencia(Transferencia transferencia)
     {
  if (!ReglasDeTransferencia.laTransferenciaEsValida(transferencia))
   return (false, "Datos de transferencia inválidos", null);

  if (!ReglasDeTransferencia.laFechaEjecucionEsValida(transferencia.FechaEjecucion))
                return (false, $"La fecha de ejecución no puede superar {ReglasDeTransferencia.DIAS_MAXIMOS_PROGRAMACION} días", null);

    var cuentaBancariaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(transferencia.IdCuentaBancariaOrigen);
            if (cuentaBancariaOrigen == null)
        return (false, "Cuenta bancaria de origen no encontrada", null);

 if (!ReglasDeTransferencia.laCuentaBancariaEstaActiva(cuentaBancariaOrigen))
         return (false, "La cuenta bancaria de origen no está activa", null);

            var cliente = await gestionCuentaDA.obtenerCuentaPorId(cuentaBancariaOrigen.IdCuenta);
   if (!ReglasDeTransferencia.elClienteEstaActivo(cliente))
  return (false, "El cliente no está activo o no tiene permisos", null);

         return (true, string.Empty, cuentaBancariaOrigen);
     }

        private async void configurarMontosYComisiones(Transferencia transferencia, CuentaBancaria cuentaBancariaOrigen)
        {
 var cuentaBancariaDestino = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorNumeroTarjeta(transferencia.NumeroCuentaDestino);
            Moneda monedaDestino = cuentaBancariaDestino?.Moneda ?? cuentaBancariaOrigen.Moneda;

            transferencia.Comision = cuentaBancariaOrigen.Moneda == Moneda.CRC
     ? ReglasDeTransferencia.COMISION_FIJA
     : ReglasDeConversionMoneda.convertirMoneda(ReglasDeTransferencia.COMISION_FIJA, Moneda.CRC, Moneda.USD);

      transferencia.MontoTotal = transferencia.Monto + transferencia.Comision;
        }

    private async Task<(bool esValido, string mensaje)> validarMontosYLimites(Transferencia transferencia, CuentaBancaria cuentaBancariaOrigen)
        {
            if (!ReglasDeTransferencia.elSaldoEsSuficiente(cuentaBancariaOrigen.Saldo, transferencia.MontoTotal))
          return (false, "Saldo insuficiente para realizar la transferencia");

            var transferenciasDelDia = await gestionTransferenciaDA.obtenerTransferenciasDelDia(transferencia.IdCuentaBancariaOrigen);
            if (!ReglasDeTransferencia.noSuperaLimiteDiario(transferenciasDelDia, transferencia.MontoTotal))
  return (false, "Se ha excedido el límite diario de transferencias");

      return (true, string.Empty);
  }

        private void configurarSaldosYEstado(Transferencia transferencia, CuentaBancaria cuentaBancariaOrigen)
        {
transferencia.SaldoAnterior = cuentaBancariaOrigen.Saldo;
            transferencia.SaldoPosterior = ReglasDeTransferencia.calcularSaldoPosterior(cuentaBancariaOrigen.Saldo, transferencia.MontoTotal);
            transferencia.FechaCreacion = DateTime.Now;
          transferencia.Estado = ReglasDeTransferencia.determinarEstadoInicial(transferencia.Monto, transferencia.FechaEjecucion);
        }

        private async Task<(bool exito, string mensaje)> ejecutarSiCorresponde(Transferencia transferencia, int referenciaId)
        {
if (transferencia.Estado == EstadoTra.Pendiente &&
              !ReglasDeTransferencia.requiereAprobacion(transferencia.Monto) &&
    transferencia.FechaEjecucion.Date == DateTime.Now.Date)
            {
       return await ejecutarTransferencia(referenciaId);
            }

      return (true, string.Empty);
        }

     private string generarMensajeConfirmacion(Transferencia transferencia)
        {
            return transferencia.Estado switch
            {
      EstadoTra.Pendiente => $"Transferencia pendiente de aprobación (supera ?{ReglasDeTransferencia.UMBRAL_APROBACION:N0})",
    EstadoTra.Programada => $"Transferencia programada para {transferencia.FechaEjecucion:dd/MM/yyyy}",
  EstadoTra.Exitosa => "Transferencia realizada exitosamente",
         _ => "Transferencia registrada"
 };
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

        public Task<List<Transferencia>> obtenerTransferenciasPorCliente(int idCliente)
        {
   return gestionTransferenciaDA.obtenerTransferenciasPorCliente(idCliente);
     }

     public Task<List<TransferenciaRecibida>> obtenerTransferenciasRecibidas(int idCliente)
  {
        return gestionTransferenciaDA.obtenerTransferenciasRecibidas(idCliente);
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

        public async Task<(bool exito, string mensaje)> aprobarTransferencia(int referencia)
 {
     var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
  if (transferencia == null)
      return (false, "Transferencia no encontrada");

   // Validar que esté pendiente
if (transferencia.Estado != EstadoTra.Pendiente)
    return (false, "Solo se pueden aprobar transferencias pendientes");

   // Ejecutar la transferencia
  var resultadoEjecucion = await ejecutarTransferencia(referencia);
      if (!resultadoEjecucion.exito)
     return (false, resultadoEjecucion.mensaje);

            // Actualizar estado a Exitosa
      await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Exitosa);
     return (true, "Transferencia aprobada y ejecutada exitosamente");
 }

     public async Task<(bool exito, string mensaje)> rechazarTransferencia(int referencia, string motivo)
   {
 var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
      if (transferencia == null)
   return (false, "Transferencia no encontrada");

  // Validar que esté pendiente
   if (transferencia.Estado != EstadoTra.Pendiente)
   return (false, "Solo se pueden rechazar transferencias pendientes");

     // Actualizar estado a Rechazada
   await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Rechazada, null, motivo);
 return (true, "Transferencia rechazada");
        }

 // Método privado para ejecutar una transferencia
     private async Task<(bool exito, string mensaje)> ejecutarTransferencia(int referencia)
        {
            var transferencia = await gestionTransferenciaDA.obtenerTransferenciaPorReferencia(referencia);
 if (transferencia == null)
       return (false, "Transferencia no encontrada");

     var cuentaBancariaOrigen = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(transferencia.IdCuentaBancariaOrigen);
 if (cuentaBancariaOrigen == null)
                return (false, "Cuenta bancaria de origen no encontrada");

        registrarLogEjecucion(referencia, transferencia, cuentaBancariaOrigen);

   var resultadoDebito = await debitarCuentaOrigen(cuentaBancariaOrigen, transferencia);
            if (!resultadoDebito)
       return (false, "Error al debitar saldo de cuenta origen");

            await acreditarCuentaDestino(transferencia, cuentaBancariaOrigen);

            await gestionTransferenciaDA.actualizarEstado(referencia, (int)EstadoTra.Exitosa);
            return (true, "Transferencia ejecutada exitosamente");
        }

  private void registrarLogEjecucion(int referencia, Transferencia transferencia, CuentaBancaria cuentaBancariaOrigen)
 {
  Console.WriteLine($"? Ejecutando transferencia #{referencia}");
    Console.WriteLine($"   Cuenta Origen: {cuentaBancariaOrigen.NumeroTarjeta} ({cuentaBancariaOrigen.Moneda})");
       Console.WriteLine($"   Monto a transferir: {transferencia.Monto}");
        }

      private async Task<bool> debitarCuentaOrigen(CuentaBancaria cuentaBancariaOrigen, Transferencia transferencia)
   {
            cuentaBancariaOrigen.Saldo = transferencia.SaldoPosterior;
     cuentaBancariaOrigen.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancariaOrigen.Saldo, cuentaBancariaOrigen.Estado);

   bool saldoActualizado = await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancariaOrigen, cuentaBancariaOrigen.Id);
 
       if (saldoActualizado)
          Console.WriteLine($"   ? Debitado de cuenta origen: {transferencia.MontoTotal} (monto + comisión)");

     return saldoActualizado;
     }

        private async Task acreditarCuentaDestino(Transferencia transferencia, CuentaBancaria cuentaBancariaOrigen)
        {
            var cuentaBancariaDestino = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorNumeroTarjeta(transferencia.NumeroCuentaDestino);
     
      if (cuentaBancariaDestino == null)
     {
      Console.WriteLine($"   ?? Cuenta destino no existe en el sistema (transferencia externa)");
    return;
     }

            long montoAcreditar = calcularMontoAcreditar(transferencia, cuentaBancariaOrigen, cuentaBancariaDestino);
   await actualizarSaldoDestino(cuentaBancariaDestino, montoAcreditar);
        }

 private long calcularMontoAcreditar(Transferencia transferencia, CuentaBancaria cuentaBancariaOrigen, CuentaBancaria cuentaBancariaDestino)
      {
       Console.WriteLine($"   Cuenta Destino: {cuentaBancariaDestino.NumeroTarjeta} ({cuentaBancariaDestino.Moneda})");

        long montoAcreditar = ReglasDeTransferencia.calcularMontoDestino(
            transferencia.Monto,
  cuentaBancariaOrigen.Moneda,
        cuentaBancariaDestino.Moneda);

  Console.WriteLine($"   Moneda Origen: {cuentaBancariaOrigen.Moneda}, Moneda Destino: {cuentaBancariaDestino.Moneda}");
            Console.WriteLine($"   Monto Original: {transferencia.Monto}");
            Console.WriteLine($"   Monto a Acreditar (después de conversión): {montoAcreditar}");

 return montoAcreditar;
        }

        private async Task actualizarSaldoDestino(CuentaBancaria cuentaBancariaDestino, long montoAcreditar)
        {
      cuentaBancariaDestino.Saldo += montoAcreditar;
            cuentaBancariaDestino.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancariaDestino.Saldo, cuentaBancariaDestino.Estado);

            bool saldoDestinoActualizado = await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancariaDestino, cuentaBancariaDestino.Id);
 
            if (!saldoDestinoActualizado)
    throw new Exception("Error al acreditar saldo en cuenta destino");

        Console.WriteLine($"   ? Acreditado a cuenta destino: {montoAcreditar}");
        }
    }
}
