using FluentHtml.Fluent;
using System.Diagnostics;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#endif

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Clase base abstracta para controles de entrada HTML seguros y fuertemente tipados.
    /// Proporciona integración con metadatos de modelo, tipo de entrada, valor y formato.
    /// </summary>
    public abstract class InputBase : SecureViewBase, IHasModelData
    {
        /// <summary>
        /// Campo privado para almacenar la metadata del modelo asociada al control.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if !NETCOREAPP && !NETSTANDARD
            private ModelMetadata? _metadata;
#else
        private ModelExplorer? _metadata;
#endif

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InputBase"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        protected InputBase(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
        }

        /// <summary>
        /// Obtiene o establece la metadata del modelo asociada al control.
        /// Al establecerse, también actualiza el valor del control con el valor del modelo.
        /// </summary>
#if !NETCOREAPP && !NETSTANDARD
            public ModelMetadata? Metadata 
#else
        public ModelExplorer? Metadata
#endif
        {
            get
            {
                return this._metadata;
            }
            set
            {
                if (this._metadata != value)
                {
                    this._metadata = value;
                    if (this._metadata is not null)
                        this.Value = this._metadata.Model;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de entrada HTML (por ejemplo, text, password, checkbox).
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// Obtiene o establece el valor del control de entrada.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Obtiene o establece el formato de visualización para el valor del control.
        /// </summary>
        public string? Format { get; set; }
    }
}
