using System;

namespace FluentHtml.Extensions
{
    /// <summary>
    /// Métodos de extensión utilitarios para manipulación y validación de cadenas de texto (<see cref="string"/>).
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Trunca el texto especificado a una longitud máxima y agrega una cadena de puntos suspensivos si es necesario.
        /// </summary>
        /// <param name="text">Texto a truncar.</param>
        /// <param name="keep">Cantidad de caracteres a conservar.</param>
        /// <param name="ellipsis">Cadena de puntos suspensivos a usar al truncar (por defecto "...").</param>
        /// <returns>
        /// Una cadena truncada con o sin puntos suspensivos, según corresponda.
        /// </returns>
        public static string Truncate(this string text, int keep, string ellipsis = "...")
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (string.IsNullOrEmpty(ellipsis))
                ellipsis = string.Empty;

            string buffer = text;
            if (buffer.Length <= keep)
                return buffer;

            if (buffer.Length <= keep + ellipsis.Length || keep < ellipsis.Length)
                return buffer.Substring(0, keep);

            return string.Concat(buffer.Substring(0, keep - ellipsis.Length), ellipsis);
        }

        /// <summary>
        /// Indica si el objeto <see cref="string"/> especificado es nulo o una cadena vacía.
        /// </summary>
        /// <param name="item">Referencia a una cadena.</param>
        /// <returns>
        /// <c>true</c> si es nulo o vacío; de lo contrario, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string item)
        {
            return string.IsNullOrEmpty(item);
        }

        /// <summary>
        /// Indica si una cadena especificada es nula, vacía o solo contiene caracteres de espacio en blanco.
        /// </summary>
        /// <param name="item">Referencia a una cadena.</param>
        /// <returns>
        /// <c>true</c> si es nula, vacía o solo contiene espacios en blanco; de lo contrario, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(this string item)
        {
            if (item is null)
                return true;

            for (int i = 0; i < item.Length; i++)
                if (!char.IsWhiteSpace(item[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Determina si la cadena especificada no es nula ni vacía.
        /// </summary>
        /// <param name="value">Valor a comprobar.</param>
        /// <returns>
        /// <c>true</c> si <paramref name="value"/> no es nulo ni vacío; de lo contrario, <c>false</c>.
        /// </returns>
        public static bool HasValue(this string? value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Usa la cadena como formato y aplica los argumentos especificados.
        /// </summary>
        /// <param name="format">Cadena de formato.</param>
        /// <param name="args">Parámetros de objeto que se deben formatear.</param>
        /// <returns>Cadena formateada.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="format"/> es <c>null</c>.</exception>
        public static string FormatWith(this string format, params object[] args)
        {
            if (format is null)
                throw new ArgumentNullException(nameof(format));

            return string.Format(format, args);
        }
    }
}