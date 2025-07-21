#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif
namespace FluentHtml.Fluent
{
    /// <summary>
    /// Define la interfaz para componentes que exponen información de nombre y metadatos del modelo,
    /// facilitando la integración con la infraestructura de binding y validación de MVC.
    /// </summary>
    public interface IHasModelData
    {
        /// <summary>
        /// Obtiene o establece el nombre completo del campo o propiedad del modelo asociado al componente.
        /// </summary>
        string? Name
        {
            get;
            set;
        }

#if !NETCOREAPP && !NETSTANDARD
        /// <summary>
        /// Obtiene o establece la metadata del modelo asociada al componente (MVC clásico).
        /// </summary>
        ModelMetadata? Metadata
#else
        /// <summary>
        /// Obtiene o establece el explorador de modelo asociado al componente (ASP.NET Core).
        /// </summary>
        ModelExplorer? Metadata
#endif
        {
            get;
            set;
        }
    }
}
