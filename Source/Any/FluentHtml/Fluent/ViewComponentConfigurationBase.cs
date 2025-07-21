using System.Collections.Generic;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para la configuración de componentes de vista fluida.
    /// Proporciona propiedades comunes como el nombre del componente y los atributos HTML asociados.
    /// </summary>
    public abstract class ViewComponentConfigurationBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ViewComponentConfigurationBase"/>.
        /// </summary>
        protected ViewComponentConfigurationBase()
        {
            HtmlAttributes = new RouteValueDictionary();
        }

        /// <summary>
        /// Obtiene o establece el nombre del componente de vista.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Obtiene el diccionario de atributos HTML personalizados para el componente.
        /// </summary>
        public IDictionary<string, object?> HtmlAttributes
        {
            get;
            private set;
        }

    }
}