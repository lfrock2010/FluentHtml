using System;
using System.Collections.Generic;

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para fábricas de colecciones de componentes, proporcionando la infraestructura para agregar elementos a una colección y construirlos de forma fluida.
    /// </summary>
    /// <typeparam name="TComponent">Tipo del componente que hereda de <see cref="ComponentBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto que hereda de <see cref="BuilderBase{TComponent, TBuilder}"/>.</typeparam>
    public abstract class CollectionFactoryBase<TComponent, TBuilder>
        where TBuilder : BuilderBase<TComponent, TBuilder>
        where TComponent : ComponentBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CollectionFactoryBase{TComponent, TBuilder}"/> con la colección especificada.
        /// </summary>
        /// <param name="collection">Colección de componentes donde se agregarán los elementos creados.</param>
        protected CollectionFactoryBase(ICollection<TComponent> collection)
        {
            Collection = collection;
        }

        /// <summary>
        /// Obtiene la colección de componentes asociada a la fábrica.
        /// </summary>
        protected ICollection<TComponent> Collection { get; private set; }

        /// <summary>
        /// Agrega un nuevo componente a la colección y devuelve su builder asociado.
        /// </summary>
        /// <returns>Instancia del builder para el nuevo componente agregado.</returns>
        public abstract TBuilder Add();
    }
}