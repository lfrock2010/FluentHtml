using System;
using System.Collections.Generic;

namespace FluentHtml.Extensions
{
    /// <summary>
    /// Métodos de extensión para colecciones que implementan <see cref="ICollection{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Agrega una secuencia de elementos a una colección que implementa <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">Tipo de los elementos de la colección.</typeparam>
        /// <param name="list">Colección de destino donde se agregarán los elementos.</param>
        /// <param name="range">Secuencia de elementos a agregar.</param>
        public static void AddRange<TSource>(this ICollection<TSource> list, IEnumerable<TSource> range)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            if (range is null)
                throw new ArgumentNullException(nameof(range));

            foreach (var r in range)
                list.Add(r);
        }

    }
}
