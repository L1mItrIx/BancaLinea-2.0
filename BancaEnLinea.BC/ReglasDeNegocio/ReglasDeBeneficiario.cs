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
            {
   Console.WriteLine("? Beneficiario es null");
    return false;
   }

 // Validar alias (no vacío, longitud entre 3 y 30 caracteres)
       bool aliasValido = !string.IsNullOrWhiteSpace(beneficiario.Alias) &&
    beneficiario.Alias.Length >= 3 &&
     beneficiario.Alias.Length <= 30;
      
            if (!aliasValido)
                Console.WriteLine($"? Alias inválido: '{beneficiario.Alias}' (longitud: {beneficiario.Alias?.Length ?? 0}, debe ser 3-30)");

        // Validar banco (no vacío, longitud máxima 100 caracteres)
            bool bancoValido = !string.IsNullOrWhiteSpace(beneficiario.Banco) &&
 beneficiario.Banco.Length <= 100;
   
      if (!bancoValido)
      Console.WriteLine($"? Banco inválido: '{beneficiario.Banco}' (longitud: {beneficiario.Banco?.Length ?? 0}, máx 100)");

  // Validar país (no vacío, longitud máxima 50 caracteres)
  bool paisValido = !string.IsNullOrWhiteSpace(beneficiario.Pais) &&
        beneficiario.Pais.Length <= 50;
  
         if (!paisValido)
           Console.WriteLine($"? País inválido: '{beneficiario.Pais}' (longitud: {beneficiario.Pais?.Length ?? 0}, máx 50)");

            // Validar número de cuenta destino (debe ser mayor a 0 y tener entre 12 y 20 dígitos)
       string numeroCuentaStr = beneficiario.NumeroCuentaDestino.ToString();
            bool numeroCuentaValido = beneficiario.NumeroCuentaDestino > 0 &&
     numeroCuentaStr.Length >= 12 &&
    numeroCuentaStr.Length <= 20;
            
            if (!numeroCuentaValido)
                Console.WriteLine($"? Número de cuenta inválido: {beneficiario.NumeroCuentaDestino} (longitud: {numeroCuentaStr.Length}, debe ser 12-20 dígitos)");

   // Validar enum de moneda
            bool monedaValida = Enum.IsDefined(typeof(Moneda), beneficiario.Moneda);
         
      if (!monedaValida)
             Console.WriteLine($"? Moneda inválida: {beneficiario.Moneda}");

// Validar IdCuenta
      bool cuentaValida = beneficiario.IdCuenta > 0;
       
  if (!cuentaValida)
            Console.WriteLine($"? IdCuenta inválido: {beneficiario.IdCuenta}");

   bool esValido = aliasValido && bancoValido && paisValido && 
     numeroCuentaValido && monedaValida && cuentaValida;
            
    if (esValido)
    Console.WriteLine($"? Beneficiario válido: Alias='{beneficiario.Alias}', Cuenta={beneficiario.NumeroCuentaDestino}");
    
         return esValido;
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
   /// Valida que un cliente no tenga más de 3 beneficiarios (sin importar el estado)
    /// </summary>
    public static bool puedeAgregarBeneficiario(IEnumerable<Beneficiarios> beneficiariosExistentes, int idCuenta)
    {
        // Contar TODOS los beneficiarios del cliente (Pendientes + Activos)
   // NO contar los Inactivos (rechazados)
 int cantidadBeneficiarios = beneficiariosExistentes
    .Count(b => b.IdCuenta == idCuenta && b.Estado != EstadoP.Inactivo);

  return cantidadBeneficiarios < 3;  // Permite 0, 1, 2 (máximo 3 total)
    }
    }
}
