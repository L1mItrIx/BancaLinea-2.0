using BancaEnLinea.BC.Enums;
using BancaEnLinea.BC.Modelos;

namespace BancaEnLinea.BC.ReglasDeNegocio
{
    public static class ReglasDeBeneficiario
    {
      /// <summary>
        /// Valida que el beneficiario tenga todos los datos requeridos correctamente
/// </summary>
        public static bool elBeneficiarioEsValido(Beneficiarios beneficiario)
        {
            if (beneficiario == null)
  return false;

 // Validar alias (no vacío, longitud entre 3 y 30 caracteres)
       bool aliasValido = !string.IsNullOrWhiteSpace(beneficiario.Alias) &&
    beneficiario.Alias.Length >= 3 &&
     beneficiario.Alias.Length <= 30;

        // Validar banco (no vacío, longitud máxima 100 caracteres)
            bool bancoValido = !string.IsNullOrWhiteSpace(beneficiario.Banco) &&
 beneficiario.Banco.Length <= 100;

  // Validar país (no vacío, longitud máxima 50 caracteres)
            bool paisValido = !string.IsNullOrWhiteSpace(beneficiario.Pais) &&
        beneficiario.Pais.Length <= 50;

            // Validar número de cuenta destino (debe ser mayor a 0 y tener entre 12 y 20 dígitos)
       string numeroCuentaStr = beneficiario.NumeroCuentaDestino.ToString();
            bool numeroCuentaValido = beneficiario.NumeroCuentaDestino > 0 &&
     numeroCuentaStr.Length >= 12 &&
          numeroCuentaStr.Length <= 20;

   // Validar enum de moneda
            bool monedaValida = Enum.IsDefined(typeof(Moneda), beneficiario.Moneda);

        // Validar IdCuenta
            bool cuentaValida = beneficiario.IdCuenta > 0;

        return aliasValido && bancoValido && paisValido && 
     numeroCuentaValido && monedaValida && cuentaValida;
        }

      /// <summary>
        /// Valida que el ID del beneficiario sea válido
        /// </summary>
        public static bool elIdEsValido(int id)
        {
            return id > 0;
   }

        /// <summary>
     /// Valida que un alias de beneficiario sea único para un cliente específico
      /// </summary>
        public static bool elAliasEsUnico(IEnumerable<Beneficiarios> beneficiariosExistentes, string nuevoAlias, int idCuenta, int? idBeneficiarioActual = null)
    {
      if (string.IsNullOrWhiteSpace(nuevoAlias))
         return false;

          // Verificar si ya existe un beneficiario con el mismo alias para este cliente
            return !beneficiariosExistentes.Any(b =>
 b.Alias.Trim().ToLower() == nuevoAlias.Trim().ToLower() &&
           b.IdCuenta == idCuenta &&
           (idBeneficiarioActual == null || b.IdBeneficiario != idBeneficiarioActual)
            );
 }

        /// <summary>
        /// Valida que un número de cuenta destino no esté duplicado para el mismo cliente
/// </summary>
        public static bool elNumeroCuentaDestinoEsUnico(IEnumerable<Beneficiarios> beneficiariosExistentes, long numeroCuentaDestino, int idCuenta, int? idBeneficiarioActual = null)
    {
            // Verificar si ya existe el mismo número de cuenta destino para este cliente
            return !beneficiariosExistentes.Any(b =>
              b.NumeroCuentaDestino == numeroCuentaDestino &&
b.IdCuenta == idCuenta &&
   (idBeneficiarioActual == null || b.IdBeneficiario != idBeneficiarioActual)
            );
        }

      /// <summary>
   /// Valida que un cliente no tenga más de 3 beneficiarios en total
        /// </summary>
        public static bool puedeAgregarBeneficiario(IEnumerable<Beneficiarios> beneficiariosExistentes, int idCuenta)
        {
   int cantidadBeneficiarios = beneficiariosExistentes.Count(b => b.IdCuenta == idCuenta);
return cantidadBeneficiarios < 3;
      }
    }
}
