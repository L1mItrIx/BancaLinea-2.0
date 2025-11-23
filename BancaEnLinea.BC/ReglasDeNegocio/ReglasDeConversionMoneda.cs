using BancaEnLinea.BC.Enums;

namespace BancaEnLinea.BC.ReglasDeNegocio
{
    /// <summary>
    /// Reglas de negocio para la conversión de moneda entre CRC y USD
    /// </summary>
    public static class ReglasDeConversionMoneda
    {
 // Tipo de cambio fijo (en producción esto vendría de un servicio externo)
        // 1 USD = 520 CRC (valor ejemplo)
        public const decimal TIPO_CAMBIO_USD_A_CRC = 520m;
        public const decimal TIPO_CAMBIO_CRC_A_USD = 1 / 520m;


   public static long convertirMoneda(long monto, Moneda monedaOrigen, Moneda monedaDestino)
        {
     // Si son la misma moneda, no hay conversión
          if (monedaOrigen == monedaDestino)
          {
    Console.WriteLine($"?? Sin conversión necesaria ({monedaOrigen} ? {monedaDestino}): {monto}");
              return monto;
          }

            // Convertir de USD a CRC
       if (monedaOrigen == Moneda.USD && monedaDestino == Moneda.CRC)
         {
         decimal montoConvertido = monto * TIPO_CAMBIO_USD_A_CRC;
     long resultado = (long)Math.Round(montoConvertido, 0, MidpointRounding.AwayFromZero);
        Console.WriteLine($"?? Conversión USD ? CRC:");
        Console.WriteLine($"   Monto USD: {monto}");
   Console.WriteLine($"   Tipo de cambio: {TIPO_CAMBIO_USD_A_CRC}");
        Console.WriteLine($"   Resultado CRC: {resultado}");
    return resultado;
       }

       // Convertir de CRC a USD
   if (monedaOrigen == Moneda.CRC && monedaDestino == Moneda.USD)
            {
            decimal montoConvertido = monto * TIPO_CAMBIO_CRC_A_USD;
            long resultado = (long)Math.Round(montoConvertido, 0, MidpointRounding.AwayFromZero);
        Console.WriteLine($"?? Conversión CRC ? USD:");
        Console.WriteLine($"   Monto CRC: {monto}");
        Console.WriteLine($"   Tipo de cambio: {TIPO_CAMBIO_CRC_A_USD}");
        Console.WriteLine($"   Resultado USD: {resultado}");
    return resultado;
            }

    Console.WriteLine($"?? Conversión no soportada: {monedaOrigen} ? {monedaDestino}");
    return monto;
        }

        /// <summary>
        /// Valida que el monto convertido sea válido (mayor a 0)
        /// </summary>
        public static bool elMontoConvertidoEsValido(long montoConvertido)
        {
 return montoConvertido > 0;
        }

        /// <summary>
 /// Obtiene el tipo de cambio actual de USD a CRC
      /// </summary>
    public static decimal obtenerTipoCambioUsdACrc()
        {
   return TIPO_CAMBIO_USD_A_CRC;
        }

  /// <summary>
 /// Obtiene el tipo de cambio actual de CRC a USD
        /// </summary>
        public static decimal obtenerTipoCambioCrcAUsd()
        {
          return TIPO_CAMBIO_CRC_A_USD;
        }

        /// <summary>
        /// Determina si se requiere conversión de moneda
        /// </summary>
        public static bool requiereConversion(Moneda monedaOrigen, Moneda monedaDestino)
        {
       return monedaOrigen != monedaDestino;
        }
    }
}
