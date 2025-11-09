using BancaEnLinea.BC.Modelos;
using BancaEnLinea.BW.Interfaces.DA;
using BancaEnLinea.DA.Config;
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

    public async Task<bool> actualizarCuenta(Cuenta cuenta, int id)
    {
      var CuentaExistente = await bancaEnLineaContext.Cuenta.FindAsync(id);
      if (CuentaExistente == null)
        return false;

      CuentaExistente.Nombre = cuenta.Nombre;
      CuentaExistente.PrimerApellido = cuenta.PrimerApellido;
      CuentaExistente.SegundoApellido = cuenta.SegundoApellido;
      CuentaExistente.Correo = cuenta.Correo;
      CuentaExistente.Contrasena = cuenta.Contrasena;
      CuentaExistente.Rol = cuenta.Rol;
      await bancaEnLineaContext.SaveChangesAsync();
      return true;
    }

    public async Task<bool> eliminarCuenta(int id)
    {
      var cuenta = await bancaEnLineaContext.Cuenta.FindAsync(id);
      if (cuenta == null)
        return false;
      bancaEnLineaContext.Cuenta.Remove(cuenta);
      bancaEnLineaContext.SaveChangesAsync();
      return true;
    }

    public Task<List<Cuenta>> obtenerCuentas()
    {
      return bancaEnLineaContext.Cuenta.ToListAsync();
    }

    public async Task<bool> registrarCuenta(Cuenta cuenta)
    {
      try
      {
        bancaEnLineaContext.Cuenta.Add(cuenta);
        await bancaEnLineaContext.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public async Task<bool> validarCuenta(string correo, string contrasena)
    {
      if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
        return false;

      try
      {
        var cuenta = await bancaEnLineaContext.Cuenta
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Correo.ToLower() == correo.ToLower());

        if (cuenta == null)
          return false;

        return cuenta.Contrasena == contrasena;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
