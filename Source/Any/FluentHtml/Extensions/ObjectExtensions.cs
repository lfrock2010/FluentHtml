using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FluentHtml.Extensions
{
    /// <summary>
    /// Proporciona métodos de extensión para objetos, facilitando operaciones comunes sobre instancias de <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convierte un objeto en un diccionario donde las claves son los nombres de las propiedades
        /// (reemplazando guiones bajos por guiones medios) y los valores son los valores de dichas propiedades.
        /// </summary>
        /// <param name="object">El objeto a convertir en diccionario. No puede ser <c>null</c>.</param>
        /// <returns>
        /// Un diccionario con los nombres de las propiedades como claves y los valores correspondientes.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="object"/> es <c>null</c>.</exception>
        public static IDictionary<string, object?> ToDictionary(this object @object)
        {
            if (@object is null)
                throw new ArgumentNullException(nameof(@object));

            var dictionary = new Dictionary<string, object?>(StringComparer.CurrentCultureIgnoreCase);
            if (@object is null)
                return dictionary;

            foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(@object))
            {
                if (property is null)
                    continue;

                dictionary.Add(property.Name.Replace("_", "-"), property.GetValue(@object));
            }

            return dictionary;
        }
    }
}