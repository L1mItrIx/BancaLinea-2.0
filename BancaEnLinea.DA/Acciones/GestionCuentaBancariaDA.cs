using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionCuentaBancariaDA : IGestionCuentaBancariaDA
    {
        private readonly BancaEnLineaContext bancaEnLineaContext;

        public GestionCuentaBancariaDA(BancaEnLineaContext bancaEnLineaContext)
        {
            this.bancaEnLineaContext = bancaEnLineaContext;
        }

        public async Task<bool> registrarCuentaBancaria(CuentaBancaria cuentaBancaria, int idCuenta)
        {
            try
            {
                var cuentaBancariaDA = new CuentaBancariaDA
                {
                    NumeroTarjeta = cuentaBancaria.NumeroTarjeta,
                    Tipo = cuentaBancaria.Tipo,
                    Moneda = cuentaBancaria.Moneda,
                    Saldo = cuentaBancaria.Saldo,
                    Estado = cuentaBancaria.Estado,
                    IdCuenta = idCuenta
                };

                bancaEnLineaContext.CuentaBancaria.Add(cuentaBancariaDA);
                await bancaEnLineaContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<CuentaBancaria>> obtenerCuentasBancarias(int idCuenta)
        {
            var cuentasDA = await bancaEnLineaContext.CuentaBancaria
              .Where(cb => cb.IdCuenta == idCuenta)
              .AsNoTracking()
              .ToListAsync();

            return cuentasDA.Select(cb => new CuentaBancaria
            {
                Id = cb.Id,
                NumeroTarjeta = cb.NumeroTarjeta,
                Tipo = cb.Tipo,
                Moneda = cb.Moneda,
                Saldo = cb.Saldo,
                Estado = cb.Estado
            }).ToList();
        }

        public async Task<List<CuentaBancaria>> obtenerTodasLasCuentasBancarias()
        {
            var cuentasDA = await bancaEnLineaContext.CuentaBancaria
              .AsNoTracking()
              .ToListAsync();

            return cuentasDA.Select(cb => new CuentaBancaria
            {
                Id = cb.Id,
                NumeroTarjeta = cb.NumeroTarjeta,
                Tipo = cb.Tipo,
                Moneda = cb.Moneda,
                Saldo = cb.Saldo,
                Estado = cb.Estado
            }).ToList();
        }

        public async Task<bool> actualizarCuentaBancaria(CuentaBancaria cuentaBancaria, int id)
        {
            var cuentaBancariaExistente = await bancaEnLineaContext.CuentaBancaria.FindAsync(id);
            if (cuentaBancariaExistente == null)
                return false;

            cuentaBancariaExistente.NumeroTarjeta = cuentaBancaria.NumeroTarjeta;
            cuentaBancariaExistente.Tipo = cuentaBancaria.Tipo;
            cuentaBancariaExistente.Moneda = cuentaBancaria.Moneda;
            cuentaBancariaExistente.Saldo = cuentaBancaria.Saldo;
            cuentaBancariaExistente.Estado = cuentaBancaria.Estado;

            await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> eliminarCuentaBancaria(int id)
        {
            var cuentaBancaria = await bancaEnLineaContext.CuentaBancaria.FindAsync(id);
            if (cuentaBancaria == null)
                return false;

            bancaEnLineaContext.CuentaBancaria.Remove(cuentaBancaria);
            await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<CuentaBancaria?> obtenerCuentaBancariaPorId(int id)
        {
            var cuentaBancariaDA = await bancaEnLineaContext.CuentaBancaria
              .AsNoTracking()
              .FirstOrDefaultAsync(cb => cb.Id == id);

            if (cuentaBancariaDA == null)
                return null;

            return new CuentaBancaria
            {
                Id = cuentaBancariaDA.Id,
                NumeroTarjeta = cuentaBancariaDA.NumeroTarjeta,
                Tipo = cuentaBancariaDA.Tipo,
                Moneda = cuentaBancariaDA.Moneda,
                Saldo = cuentaBancariaDA.Saldo,
                Estado = cuentaBancariaDA.Estado
            };
        }
    }
}