using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.ReglasDeNegocio;
using BancaEnLinea.BW.Interfaces.BW;
using BancaEnLinea.BW.Interfaces.DA;

namespace BancaEnLinea.BW.CU
{
    public class GestionPagoServicioBW : IGestionPagoServicioBW
    {
private readonly IGestionPagoServicioDA gestionPagoServicioDA;
        private readonly IGestionCuentaBancariaDA gestionCuentaBancariaDA;
private readonly IGestionContratoServicioDA gestionContratoServicioDA;

        public GestionPagoServicioBW(
            IGestionPagoServicioDA gestionPagoServicioDA,
       IGestionCuentaBancariaDA gestionCuentaBancariaDA,
 IGestionContratoServicioDA gestionContratoServicioDA)
 {
 this.gestionPagoServicioDA = gestionPagoServicioDA;
     this.gestionCuentaBancariaDA = gestionCuentaBancariaDA;
   this.gestionContratoServicioDA = gestionContratoServicioDA;
 }

      public async Task<(bool exito, string mensaje, int? idPago)> realizarPago(PagoServicioRequest request)
 {
   // Validar contrato
    var contrato = await gestionContratoServicioDA.obtenerContratoPorNumero(request.NumeroContrato);
       if (contrato == null)
   return (false, "Número de contrato no encontrado", null);

    // Validar cuenta bancaria
 var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(request.IdCuentaBancariaOrigen);
   if (cuentaBancaria == null)
       return (false, "Cuenta bancaria no encontrada", null);

  if (cuentaBancaria.Estado != EstadoCB.Activa)
   return (false, "La cuenta bancaria no está activa", null);

     // Calcular montos con conversión de moneda si es necesario
     const long COMISION_PAGO_SERVICIO_CRC = 300;
            
       // La comisión se calcula en la moneda de la cuenta origen
       long comision = cuentaBancaria.Moneda == Moneda.CRC ? 
    COMISION_PAGO_SERVICIO_CRC : 
  ReglasDeConversionMoneda.convertirMoneda(COMISION_PAGO_SERVICIO_CRC, Moneda.CRC, Moneda.USD);
  
          // El monto del pago puede necesitar conversión
   // Si la cuenta es USD, el monto ya debe venir en USD desde el cliente
       // Aquí asumimos que el monto viene en la moneda de la cuenta origen
   long montoTotal = request.Monto + comision;

    // Validar saldo
  if (cuentaBancaria.Saldo < montoTotal)
     return (false, "Saldo insuficiente", null);

   // Determinar fecha de ejecución
   DateTime fechaEjecucion = request.FechaEjecucion ?? DateTime.Now;
      
  // Determinar estado
     EstadoTra estado;
if (fechaEjecucion.Date > DateTime.Now.Date)
     estado = EstadoTra.Programada;
 else
 estado = EstadoTra.Pendiente;

   // Crear pago
  var pago = new PagoServicio
   {
   IdContratoServicio = contrato.IdContratoServicio,
   IdCuentaBancariaOrigen = request.IdCuentaBancariaOrigen,
     Monto = request.Monto,
Comision = comision,
    MontoTotal = montoTotal,
   FechaCreacion = DateTime.Now,
   FechaEjecucion = fechaEjecucion,
 Estado = estado
  };

       int idPago = await gestionPagoServicioDA.registrarPago(pago);
if (idPago <= 0)
   return (false, "Error al registrar el pago", null);

  // Si es inmediato, ejecutar
   if (estado == EstadoTra.Pendiente)
   {
 // Debitar saldo
   cuentaBancaria.Saldo -= montoTotal;
    cuentaBancaria.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(cuentaBancaria.Saldo, cuentaBancaria.Estado);
    await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancaria, cuentaBancaria.Id);

        // Actualizar estado
await gestionPagoServicioDA.actualizarEstado(idPago, (int)EstadoTra.Exitosa);

   return (true, "Pago realizado exitosamente", idPago);
   }

   return (true, $"Pago programado para {fechaEjecucion:dd/MM/yyyy}", idPago);
        }

     public Task<List<PagoServicio>> obtenerPagosPorCliente(int idCliente)
        {
        return gestionPagoServicioDA.obtenerPagosPorCliente(idCliente);
    }

        public Task<List<PagoServicio>> obtenerPagosProgramados(int idCliente)
{
      return gestionPagoServicioDA.obtenerPagosProgramados(idCliente);
}

public Task<List<PagoServicio>> obtenerTodosPagos()
{
    return gestionPagoServicioDA.obtenerTodosPagos();
}

public async Task<(bool exito, string mensaje)> cancelarPago(int idPago, int idCliente)
 {
       var pago = await gestionPagoServicioDA.obtenerPagoPorId(idPago);
  if (pago == null)
  return (false, "Pago no encontrado");

   // Validar que sea del cliente
    var cuentaBancaria = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(pago.IdCuentaBancariaOrigen);
   if (cuentaBancaria?.IdCuenta != idCliente)
   return (false, "No tiene permisos para cancelar este pago");

 // Validar que sea programado
      if (pago.Estado != EstadoTra.Programada)
return (false, "Solo se pueden cancelar pagos programados");

 // Validar 24 horas
 TimeSpan diferencia = pago.FechaEjecucion - DateTime.Now;
    if (diferencia.TotalHours < 24)
   return (false, "Solo se puede cancelar con al menos 24 horas de anticipación");

    bool resultado = await gestionPagoServicioDA.cancelarPago(idPago);
  return resultado 
        ? (true, "Pago cancelado exitosamente")
: (false, "Error al cancelar el pago");
  }
    }
}
