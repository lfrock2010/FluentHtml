using System.Collections.Generic;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using RequestContext = Microsoft.AspNetCore.Mvc.Rendering.FluentRequestContext;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Define la interfaz base para los componentes de vista fluida, proporcionando propiedades y métodos esenciales para la integración con el contexto de la vista y la gestión de atributos HTML.
    /// </summary>
    public interface IViewComponentBase
    {
        /// <summary>
        /// Obtiene el contexto de la petición asociado al componente.
        /// </summary>
        RequestContext RequestContext
        {
            get;
        }

        /// <summary>
        /// Obtiene el contexto de la vista asociado al componente.
        /// </summary>
        ViewContext ViewContext
        {
            get;
        }

        /// <summary>
        /// Obtiene o establece el nombre completo del campo HTML asociado al componente.
        /// </summary>
        string? Name
        {
            get;
            set;
        }

        /// <summary>
        /// Obtiene el conjunto de clases CSS asociadas al componente.
        /// </summary>
        ISet<string> CssClasses
        {
            get;
        }

        /// <summary>
        /// Verifica que la configuración del componente sea válida.
        /// Debe lanzar una excepción si la configuración es incorrecta.
        /// </summary>
        void VerifySettings();

        /// <summary>
        /// Establece el contexto de la vista para el componente.
        /// </summary>
        /// <param name="viewContext">El nuevo contexto de la vista.</param>
        void SetViewContext(ViewContext viewContext);
    }
}
