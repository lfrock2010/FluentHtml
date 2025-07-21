using System;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Representa una solicitud de navegación para la generación de enlaces o redirecciones,
    /// permitiendo especificar acción, controlador, ruta, valores de ruta y URL directa.
    /// </summary>
    public class NavigationRequest
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NavigationRequest"/>.
        /// </summary>
        public NavigationRequest()
        {
            RouteValues = new RouteValueDictionary();
        }

        /// <summary>
        /// Obtiene o establece el nombre de la ruta utilizada para la navegación.
        /// </summary>
        /// <value>Nombre de la ruta.</value>
        public string? RouteName { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del controlador utilizado para la navegación.
        /// </summary>
        /// <value>Nombre del controlador.</value>
        public string? ControllerName { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la acción utilizada para la navegación.
        /// </summary>
        /// <value>Nombre de la acción.</value>
        public string? ActionName { get; set; }

        /// <summary>
        /// Obtiene el diccionario de valores de ruta asociados a la navegación.
        /// </summary>
        /// <value>Valores de ruta.</value>
        public RouteValueDictionary RouteValues { get; private set; }

        /// <summary>
        /// Obtiene o establece la URL directa para la navegación.
        /// Si se especifica, tiene prioridad sobre acción, controlador y ruta.
        /// </summary>
        /// <value>URL de destino.</value>
        public string? Url { get; set; }
    }
}
