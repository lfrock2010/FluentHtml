#if NETCOREAPP || NETSTANDARD
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    /// <summary>
    /// Contexto de solicitud para FluentHtml, encapsula el <see cref="HttpContext"/> y <see cref="RouteData"/>
    /// asociados a la petición actual en ASP.NET Core.
    /// </summary>
    public class FluentRequestContext
    {
        /// <summary>
        /// Obtiene el contexto HTTP actual de la solicitud.
        /// </summary>
        public HttpContext HttpContext
        {
            get;
        }

        /// <summary>
        /// Obtiene los datos de enrutamiento (<see cref="RouteData"/>) de la solicitud actual.
        /// </summary>
        public virtual RouteData RouteData
        {
            get;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FluentRequestContext"/>.
        /// </summary>
        /// <param name="httpContext">Contexto HTTP de la solicitud.</param>
        /// <param name="routeData">Datos de enrutamiento asociados a la solicitud.</param>
        public FluentRequestContext(HttpContext httpContext, RouteData routeData)
        {
            this.HttpContext = httpContext;
            this.RouteData = routeData;
        }
    }

    /// <summary>
    /// Métodos utilitarios para trabajar con <see cref="ViewContext"/> y obtener el contexto de solicitud fluido.
    /// </summary>
    public static class ViewContextUtils
    {
        /// <summary>
        /// Obtiene una instancia de <see cref="FluentRequestContext"/> a partir de un <see cref="ViewContext"/>.
        /// </summary>
        /// <param name="viewContext">Contexto de la vista actual.</param>
        /// <returns>Instancia de <see cref="FluentRequestContext"/> con el contexto HTTP y los datos de ruta.</returns>
        public static FluentRequestContext GetRequestContext(this ViewContext viewContext)
        {
            return new FluentRequestContext
                    (
                        httpContext: viewContext.HttpContext,
                        routeData: viewContext.RouteData
                    );
        }
    }
}
#endif