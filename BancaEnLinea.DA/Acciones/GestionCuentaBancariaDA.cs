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

        public async Task<bool> registrarCuentaBancaria(CuentaBancaria cuentaBancaria, int identificadorCuenta)
        {
            try
            {
                // Mapear de CuentaBancaria (modelo de negocio) a CuentaBancariaDA (entidad)
                var cuentaBancariaEntidad = new CuentaBancariaDA
                {
                    NumeroTarjeta = cuentaBancaria.NumeroTarjeta,
                    Tipo = cuentaBancaria.Tipo,
                    Moneda = cuentaBancaria.Moneda,
                    Saldo = cuentaBancaria.Saldo,
                    Estado = cuentaBancaria.Estado,
                    IdCuenta = identificadorCuenta
                };

                bancaEnLineaContext.CuentaBancaria.Add(cuentaBancariaEntidad);
                await bancaEnLineaContext.SaveChangesAsync();
                return true;
            }
            catch (Exception excepcion)
            {
                return false;
            }
        }

        public async Task<List<CuentaBancaria>> obtenerCuentasBancarias(int identificadorCuenta)
        {
            var cuentasBancariasDeBaseDeDatos = await bancaEnLineaContext.CuentaBancaria
                .Where(cuentaBancariaEntidad => cuentaBancariaEntidad.IdCuenta == identificadorCuenta)
                .Include(cb => cb.Cuenta)
                .AsNoTracking()
                .ToListAsync();

            // Mapear de CuentaBancariaDA (entidad) a CuentaBancaria (modelo de negocio)
            return cuentasBancariasDeBaseDeDatos.Select(cuentaBancariaEntidad => new CuentaBancaria
            {
                Id = cuentaBancariaEntidad.Id,
                NumeroTarjeta = cuentaBancariaEntidad.NumeroTarjeta,
                Tipo = cuentaBancariaEntidad.Tipo,
                Moneda = cuentaBancariaEntidad.Moneda,
                Saldo = cuentaBancariaEntidad.Saldo,
                Estado = cuentaBancariaEntidad.Estado,
                IdCuenta = cuentaBancariaEntidad.IdCuenta,
                Cuenta = cuentaBancariaEntidad.Cuenta != null ? new Cuenta
                {
                    Id = cuentaBancariaEntidad.Cuenta.Id,
                    Nombre = cuentaBancariaEntidad.Cuenta.Nombre,
                    PrimerApellido = cuentaBancariaEntidad.Cuenta.PrimerApellido,
                    SegundoApellido = cuentaBancariaEntidad.Cuenta.SegundoApellido,
                    Correo = cuentaBancariaEntidad.Cuenta.Correo,
                    Telefono = cuentaBancariaEntidad.Cuenta.Telefono,
                    Rol = cuentaBancariaEntidad.Cuenta.Rol
                } : null
            }).ToList();
        }

        public async Task<List<CuentaBancaria>> obtenerTodasLasCuentasBancarias()
        {
            var cuentasBancariasDeBaseDeDatos = await bancaEnLineaContext.CuentaBancaria
                .Include(cb => cb.Cuenta)
                .AsNoTracking()
                .ToListAsync();

            // Mapear de CuentaBancariaDA (entidad) a CuentaBancaria (modelo de negocio)
            return cuentasBancariasDeBaseDeDatos.Select(cuentaBancariaEntidad => new CuentaBancaria
            {
                Id = cuentaBancariaEntidad.Id,
                NumeroTarjeta = cuentaBancariaEntidad.NumeroTarjeta,
                Tipo = cuentaBancariaEntidad.Tipo,
                Moneda = cuentaBancariaEntidad.Moneda,
                Saldo = cuentaBancariaEntidad.Saldo,
                Estado = cuentaBancariaEntidad.Estado,
                IdCuenta = cuentaBancariaEntidad.IdCuenta,
                Cuenta = cuentaBancariaEntidad.Cuenta != null ? new Cuenta
                {
                    Id = cuentaBancariaEntidad.Cuenta.Id,
                    Nombre = cuentaBancariaEntidad.Cuenta.Nombre,
                    PrimerApellido = cuentaBancariaEntidad.Cuenta.PrimerApellido,
                    SegundoApellido = cuentaBancariaEntidad.Cuenta.SegundoApellido,
                    Correo = cuentaBancariaEntidad.Cuenta.Correo,
                    Telefono = cuentaBancariaEntidad.Cuenta.Telefono,
                    Rol = cuentaBancariaEntidad.Cuenta.Rol
                } : null
            }).ToList();
        }

        public async Task<bool> actualizarCuentaBancaria(CuentaBancaria cuentaBancaria, int identificador)
        {
            var cuentaBancariaExistente = await bancaEnLineaContext.CuentaBancaria.FindAsync(identificador);
            if (cuentaBancariaExistente == null)
                return false;

            cuentaBancariaExistente.NumeroTarjeta = cuentaBancaria.NumeroTarjeta;
            cuentaBancariaExistente.Tipo = cuentaBancaria.Tipo;
            cuentaBancariaExistente.Moneda = cuentaBancaria.Moneda;
            cuentaBancariaExistente.Saldo = cuentaBancaria.Saldo;
            cuentaBancariaExistente.Estado = cuentaBancaria.Estado;
            // IdCuenta no se actualiza, se mantiene el original

            await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> eliminarCuentaBancaria(int identificador)
        {
            var cuentaBancaria = await bancaEnLineaContext.CuentaBancaria.FindAsync(identificador);
            if (cuentaBancaria == null)
                return false;

            bancaEnLineaContext.CuentaBancaria.Remove(cuentaBancaria);
            await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<CuentaBancaria?> obtenerCuentaBancariaPorId(int identificador)
        {
            var cuentaBancariaEntidad = await bancaEnLineaContext.CuentaBancaria
                .Include(cb => cb.Cuenta)
                .AsNoTracking()
                .FirstOrDefaultAsync(cuentaBancaria => cuentaBancaria.Id == identificador);

            if (cuentaBancariaEntidad == null)
                return null;

            // Mapear de CuentaBancariaDA (entidad) a CuentaBancaria (modelo de negocio)
            return new CuentaBancaria
            {
                Id = cuentaBancariaEntidad.Id,
                NumeroTarjeta = cuentaBancariaEntidad.NumeroTarjeta,
                Tipo = cuentaBancariaEntidad.Tipo,
                Moneda = cuentaBancariaEntidad.Moneda,
                Saldo = cuentaBancariaEntidad.Saldo,
                Estado = cuentaBancariaEntidad.Estado,
                IdCuenta = cuentaBancariaEntidad.IdCuenta,
                Cuenta = cuentaBancariaEntidad.Cuenta != null ? new Cuenta
                {
                    Id = cuentaBancariaEntidad.Cuenta.Id,
                    Nombre = cuentaBancariaEntidad.Cuenta.Nombre,
                    PrimerApellido = cuentaBancariaEntidad.Cuenta.PrimerApellido,
                    SegundoApellido = cuentaBancariaEntidad.Cuenta.SegundoApellido,
                    Correo = cuentaBancariaEntidad.Cuenta.Correo,
                    Telefono = cuentaBancariaEntidad.Cuenta.Telefono,
                    Rol = cuentaBancariaEntidad.Cuenta.Rol
                } : null
            };
        }
    }
}