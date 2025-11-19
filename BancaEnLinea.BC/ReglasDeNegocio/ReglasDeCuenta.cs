using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.Modelos;
using System.Text.RegularExpressions;

namespace BancaEnLinea.BC.ReglasDeNegocio
{
  public static class ReglasDeCuenta
  {
    public static bool laCuentaEsValida(Cuenta cuenta)
    {
      return cuenta != null &&
        elTelefonoEsValido(cuenta.Telefono) &&
        !string.IsNullOrEmpty(cuenta.Nombre) &&
        !string.IsNullOrEmpty(cuenta.PrimerApellido) &&
        !string.IsNullOrEmpty(cuenta.SegundoApellido) &&
        elCorreoEsValido(cuenta.Correo) &&
        laContrasenaEsValida(cuenta.Contrasena) &&
        Enum.IsDefined(typeof(RolCuenta), cuenta.Rol);
    }
    public static bool elIdEsValido(int id)
    {
      return id > 0;
    }
    public static bool elTelefonoEsValido(long cedula)
    {
        string telefonoStr = cedula.ToString();
        return telefonoStr.Length == 8 &&
        cedula > 0;
    }
    public static bool elCorreoEsValido(string correo)
    {
      try
      {
        var addr = new System.Net.Mail.MailAddress(correo);
        return addr.Address == correo;
      }
      catch { return false; }
    }
    public static bool laContrasenaEsValida(string contrasena)
    {
      return !string.IsNullOrEmpty(contrasena) &&
        contrasena.Length >= 8 &&
        Regex.IsMatch(contrasena, @"[a-z]") &&
        Regex.IsMatch(contrasena, @"[A-Z]") &&
        Regex.IsMatch(contrasena, @"[0-9]") &&
        Regex.IsMatch(contrasena, @"[^A-Za-z0-9]");
    }
  }
}
