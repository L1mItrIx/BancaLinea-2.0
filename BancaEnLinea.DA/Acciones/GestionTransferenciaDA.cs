using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BC.Enums;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionTransferenciaDA : IGestionTransferenciaDA
    {
 private readonly BancaEnLineaContext bancaEnLineaContext;

        public GestionTransferenciaDA(BancaEnLineaContext bancaEnLineaContext)
  {
      this.bancaEnLineaContext = bancaEnLineaContext;
 }

   public async Task<int> registrarTransferencia(Transferencia transferencia)
{
            try
            {
       var transferenciaEntidad = new TransferenciaDA
    {
        IdempotencyKey = transferencia.IdempotencyKey,
     IdCuentaBancariaOrigen = transferencia.IdCuentaBancariaOrigen,
          NumeroCuentaDestino = transferencia.NumeroCuentaDestino,
      Monto = transferencia.Monto,
          Comision = transferencia.Comision,
         MontoTotal = transferencia.MontoTotal,
SaldoAnterior = transferencia.SaldoAnterior,
    SaldoPosterior = transferencia.SaldoPosterior,
    FechaCreacion = transferencia.FechaCreacion,
   FechaEjecucion = transferencia.FechaEjecucion,
        Estado = transferencia.Estado,
       Descripcion = transferencia.Descripcion
 };

     bancaEnLineaContext.Transferencia.Add(transferenciaEntidad);
    await bancaEnLineaContext.SaveChangesAsync();
    return transferenciaEntidad.Referencia;
      }
    catch
      {
     return 0;
   }
        }

        public async Task<List<Transferencia>> obtenerTransferenciasPorCuentaBancaria(int idCuentaBancaria)
     {
          var transferencias = await bancaEnLineaContext.Transferencia
  .Where(t => t.IdCuentaBancariaOrigen == idCuentaBancaria)
       .Include(t => t.CuentaBancariaOrigen)
          .ThenInclude(cb => cb.Cuenta)
   .Include(t => t.Aprobador)
            .AsNoTracking()
    .OrderByDescending(t => t.FechaCreacion)
    .ToListAsync();

   return MapearTransferencias(transferencias);
   }

  public async Task<List<Transferencia>> obtenerTodasLasTransferencias()
      {
     var transferencias = await bancaEnLineaContext.Transferencia
  .Include(t => t.CuentaBancariaOrigen)
         .ThenInclude(cb => cb.Cuenta)
   .Include(t => t.Aprobador)
    .AsNoTracking()
    .OrderByDescending(t => t.FechaCreacion)
    .ToListAsync();

        return MapearTransferencias(transferencias);
        }

        public async Task<Transferencia?> obtenerTransferenciaPorReferencia(int referencia)
  {
    var transferencia = await bancaEnLineaContext.Transferencia
      .Include(t => t.CuentaBancariaOrigen)
    .ThenInclude(cb => cb.Cuenta)
       .Include(t => t.Aprobador)
    .AsNoTracking()
  .FirstOrDefaultAsync(t => t.Referencia == referencia);

   return transferencia != null ? MapearTransferencia(transferencia) : null;
 }

    public async Task<List<Transferencia>> obtenerTransferenciasPendientes()
        {
         var transferencias = await bancaEnLineaContext.Transferencia
       .Where(t => t.Estado == EstadoTra.Pendiente)
                .Include(t => t.CuentaBancariaOrigen)
      .ThenInclude(cb => cb.Cuenta)
 .AsNoTracking()
  .OrderBy(t => t.FechaCreacion)
      .ToListAsync();

   return MapearTransferencias(transferencias);
   }

        public async Task<List<Transferencia>> obtenerTransferenciasDelDia(int idCuentaBancaria)
 {
     var hoy = DateTime.Now.Date;
   var transferencias = await bancaEnLineaContext.Transferencia
       .Where(t => t.IdCuentaBancariaOrigen == idCuentaBancaria && t.FechaCreacion.Date == hoy)
     .AsNoTracking()
       .ToListAsync();

     return MapearTransferencias(transferencias);
 }

      public async Task<bool> actualizarEstado(int referencia, int estado, int? idAprobador = null, string? descripcion = null)
  {
         var transferencia = await bancaEnLineaContext.Transferencia.FindAsync(referencia);
   if (transferencia == null)
  return false;

            transferencia.Estado = (EstadoTra)estado;
 if (idAprobador.HasValue)
    transferencia.IdAprobador = idAprobador;
   if (!string.IsNullOrEmpty(descripcion))
   transferencia.Descripcion = descripcion;

   await bancaEnLineaContext.SaveChangesAsync();
      return true;
}

        public async Task<bool> actualizarSaldo(int referencia, long saldoPosterior)
        {
     var transferencia = await bancaEnLineaContext.Transferencia.FindAsync(referencia);
 if (transferencia == null)
           return false;

  transferencia.SaldoPosterior = saldoPosterior;
   await bancaEnLineaContext.SaveChangesAsync();
  return true;
   }

        public async Task<List<Transferencia>> obtenerTransferenciasPorCliente(int idCliente)
{
    // Obtener todos los números de cuenta bancaria del cliente
    var numerosCuentasCliente = await bancaEnLineaContext.CuentaBancaria
        .Where(cb => cb.IdCuenta == idCliente)
   .Select(cb => cb.NumeroTarjeta)
        .ToListAsync();

 // Obtener transferencias donde el cliente es ORIGEN o DESTINO
    var transferencias = await bancaEnLineaContext.Transferencia
        .Include(t => t.CuentaBancariaOrigen)
        .ThenInclude(cb => cb.Cuenta)
        .Include(t => t.Aprobador)
        .Where(t => 
// Transferencias ENVIADAS (cliente es origen)
         t.CuentaBancariaOrigen.IdCuenta == idCliente ||
         // Transferencias RECIBIDAS (cliente es destino)
            numerosCuentasCliente.Contains(t.NumeroCuentaDestino))
        .AsNoTracking()
        .OrderByDescending(t => t.FechaCreacion)
        .ToListAsync();

    return MapearTransferencias(transferencias);
 }

        // Método privado para mapear con TODA la información necesaria
 private List<Transferencia> MapearTransferencias(List<TransferenciaDA> transferenciasDA)
 {
     return transferenciasDA.Select(t => MapearTransferencia(t)).ToList();
 }

 private Transferencia MapearTransferencia(TransferenciaDA t)
  {
     return new Transferencia
 {
    Referencia = t.Referencia,
   IdempotencyKey = t.IdempotencyKey,
   IdCuentaBancariaOrigen = t.IdCuentaBancariaOrigen,
    NumeroCuentaDestino = t.NumeroCuentaDestino,
    Monto = t.Monto,
 Comision = t.Comision,
     MontoTotal = t.MontoTotal,
   SaldoAnterior = t.SaldoAnterior,
    SaldoPosterior = t.SaldoPosterior,
    FechaCreacion = t.FechaCreacion,
     FechaEjecucion = t.FechaEjecucion,
    Estado = t.Estado,
   Descripcion = t.Descripcion,
       CuentaBancariaOrigen = t.CuentaBancariaOrigen != null ? new CuentaBancaria
   {
  Id = t.CuentaBancariaOrigen.Id,
        NumeroTarjeta = t.CuentaBancariaOrigen.NumeroTarjeta,
    Tipo = t.CuentaBancariaOrigen.Tipo,
     Moneda = t.CuentaBancariaOrigen.Moneda,
   Saldo = t.CuentaBancariaOrigen.Saldo,
    Estado = t.CuentaBancariaOrigen.Estado,
  IdCuenta = t.CuentaBancariaOrigen.IdCuenta,
   Cuenta = t.CuentaBancariaOrigen.Cuenta != null ? new Cuenta
     {
     Id = t.CuentaBancariaOrigen.Cuenta.Id,
    Telefono = t.CuentaBancariaOrigen.Cuenta.Telefono,
      Nombre = t.CuentaBancariaOrigen.Cuenta.Nombre,
 PrimerApellido = t.CuentaBancariaOrigen.Cuenta.PrimerApellido,
          SegundoApellido = t.CuentaBancariaOrigen.Cuenta.SegundoApellido,
     Correo = t.CuentaBancariaOrigen.Cuenta.Correo,
     Rol = t.CuentaBancariaOrigen.Cuenta.Rol
   } : null
   } : null,
Aprobador = t.Aprobador != null ? new Cuenta
{
     Id = t.Aprobador.Id,
    Telefono = t.Aprobador.Telefono,
     Nombre = t.Aprobador.Nombre,
  PrimerApellido = t.Aprobador.PrimerApellido,
     SegundoApellido = t.Aprobador.SegundoApellido,
  Correo = t.Aprobador.Correo,
        Rol = t.Aprobador.Rol
     } : null
      };
   }

        public async Task<List<TransferenciaRecibida>> obtenerTransferenciasRecibidas(int idCliente)
        {
 // Obtener números de cuenta del cliente
       var numerosCuentasCliente = await bancaEnLineaContext.CuentaBancaria
  .Where(cb => cb.IdCuenta == idCliente)
      .Select(cb => cb.NumeroTarjeta)
    .ToListAsync();

// Obtener solo transferencias donde el cliente es DESTINO
  var transferencias = await bancaEnLineaContext.Transferencia
   .Include(t => t.CuentaBancariaOrigen)
   .ThenInclude(cb => cb.Cuenta)
      .Where(t => numerosCuentasCliente.Contains(t.NumeroCuentaDestino) && t.Estado == EstadoTra.Exitosa)
        .AsNoTracking()
  .OrderByDescending(t => t.FechaCreacion)
   .ToListAsync();

// Mapear a DTO simplificado
   return transferencias.Select(t => new TransferenciaRecibida
        {
       Referencia = t.Referencia,
  Monto = t.Monto,
      FechaRecepcion = t.FechaEjecucion,
Remitente = t.CuentaBancariaOrigen?.Cuenta != null 
  ? $"{t.CuentaBancariaOrigen.Cuenta.Nombre} {t.CuentaBancariaOrigen.Cuenta.PrimerApellido}"
: "Desconocido",
    CuentaOrigen = t.CuentaBancariaOrigen?.NumeroTarjeta ?? 0,
        Descripcion = t.Descripcion ?? "Sin descripción"
   }).ToList();
        }
    }
}
