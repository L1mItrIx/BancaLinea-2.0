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
       Descripcion = transferencia.Descripcion,
    IdAprobador = transferencia.IdAprobador
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

  // Métodos privados de mapeo
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
   IdAprobador = t.IdAprobador,
       CuentaBancariaOrigen = t.CuentaBancariaOrigen != null ? new CuentaBancaria
           {
  Id = t.CuentaBancariaOrigen.Id,
        NumeroTarjeta = t.CuentaBancariaOrigen.NumeroTarjeta,
      Tipo = t.CuentaBancariaOrigen.Tipo,
                Moneda = t.CuentaBancariaOrigen.Moneda,
        Saldo = t.CuentaBancariaOrigen.Saldo,
      Estado = t.CuentaBancariaOrigen.Estado,
    IdCuenta = t.CuentaBancariaOrigen.IdCuenta
   } : null,
            Aprobador = t.Aprobador != null ? new Cuenta
    {
           Id = t.Aprobador.Id,
     Nombre = t.Aprobador.Nombre,
  PrimerApellido = t.Aprobador.PrimerApellido,
     SegundoApellido = t.Aprobador.SegundoApellido,
  Correo = t.Aprobador.Correo,
        Rol = t.Aprobador.Rol
     } : null
      };
   }
    }
}
