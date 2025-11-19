using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
using BancaEnLinea.DA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BancaEnLinea.DA.Acciones
{
    public class GestionCuentaDA : IGestionCuentaDA
    {
        private readonly BancaEnLineaContext bancaEnLineaContext;

        public GestionCuentaDA(BancaEnLineaContext bancaEnLineaContext)
        {
            this.bancaEnLineaContext = bancaEnLineaContext;
        }

        public async Task<bool> actualizarCuenta(Cuenta cuenta, int identificador)
        {
            var cuentaExistente = await bancaEnLineaContext.Cuenta.FindAsync(identificador);
            if (cuentaExistente == null)
                return false;

            cuentaExistente.Nombre = cuenta.Nombre;
            cuentaExistente.PrimerApellido = cuenta.PrimerApellido;
            cuentaExistente.SegundoApellido = cuenta.SegundoApellido;
            cuentaExistente.Correo = cuenta.Correo;
            cuentaExistente.Contrasena = cuenta.Contrasena;
            cuentaExistente.Rol = cuenta.Rol;

            await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> eliminarCuenta(int identificador)
        {
            var cuenta = await bancaEnLineaContext.Cuenta.FindAsync(identificador);
            if (cuenta == null)
                return false;

            bancaEnLineaContext.Cuenta.Remove(cuenta);
            await bancaEnLineaContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Cuenta>> obtenerCuentas()
        {
            var cuentasDeBaseDeDatos = await bancaEnLineaContext.Cuenta.ToListAsync();

            // Mapear de CuentaDA (entidad) a Cuenta (modelo de negocio)
            return cuentasDeBaseDeDatos.Select(cuentaEntidad => new Cuenta
            {
                Id = cuentaEntidad.Id,
                Telefono = cuentaEntidad.Telefono,
                Nombre = cuentaEntidad.Nombre,
                PrimerApellido = cuentaEntidad.PrimerApellido,
                SegundoApellido = cuentaEntidad.SegundoApellido,
                Correo = cuentaEntidad.Correo,
                Contrasena = cuentaEntidad.Contrasena,
                Rol = cuentaEntidad.Rol
            }).ToList();
        }

        public async Task<bool> registrarCuenta(Cuenta cuenta)
        {
            try
            {
                // Mapear de Cuenta (modelo de negocio) a CuentaDA (entidad)
                var cuentaEntidad = new CuentaDA
                {
                    Telefono = cuenta.Telefono,
                    Nombre = cuenta.Nombre,
                    PrimerApellido = cuenta.PrimerApellido,
                    SegundoApellido = cuenta.SegundoApellido,
                    Correo = cuenta.Correo,
                    Contrasena = cuenta.Contrasena,
                    Rol = cuenta.Rol
                };

                bancaEnLineaContext.Cuenta.Add(cuentaEntidad);
                await bancaEnLineaContext.SaveChangesAsync();
                return true;
            }
            catch (Exception excepcion)
            {
                return false;
            }
        }

        public async Task<Cuenta?> validarCuenta(string correoElectronico, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correoElectronico) || string.IsNullOrWhiteSpace(contrasena))
                return null;

            try
            {
                var cuentaEntidad = await bancaEnLineaContext.Cuenta
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cuenta => cuenta.Correo.ToLower() == correoElectronico.ToLower());

                if (cuentaEntidad == null)
                    return null;

                if (cuentaEntidad.Contrasena != contrasena)
                    return null;

                return new Cuenta
                {
                    Id = cuentaEntidad.Id,
                    Telefono = cuentaEntidad.Telefono,
                    Nombre = cuentaEntidad.Nombre,
                    PrimerApellido = cuentaEntidad.PrimerApellido,
                    SegundoApellido = cuentaEntidad.SegundoApellido,
                    Correo = cuentaEntidad.Correo,
                    Contrasena = cuentaEntidad.Contrasena,
                    Rol = cuentaEntidad.Rol
                };
            }
            catch (Exception excepcion)
            {
                return null;
            }
        }

        public async Task<Cuenta?> obtenerCuentaPorId(int identificador)
        {
            try
            {
                var cuentaEntidad = await bancaEnLineaContext.Cuenta
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cuenta => cuenta.Id == identificador);

                if (cuentaEntidad == null)
                    return null;

                return new Cuenta
                {
                    Id = cuentaEntidad.Id,
                    Telefono = cuentaEntidad.Telefono,
                    Nombre = cuentaEntidad.Nombre,
                    PrimerApellido = cuentaEntidad.PrimerApellido,
                    SegundoApellido = cuentaEntidad.SegundoApellido,
                    Correo = cuentaEntidad.Correo,
                    Contrasena = cuentaEntidad.Contrasena,
                    Rol = cuentaEntidad.Rol
                };
            }
            catch (Exception excepcion)
            {
                return null;
            }
        }
    }
}