using System;
using FluentHtml.Extensions;
using FluentHtml.Fluent;
using System.IO;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
#else
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#endif

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Representa una etiqueta HTML <c>&lt;label&gt;</c> configurable y segura,
    /// permitiendo personalizar el texto, el atributo <c>for</c>, la codificación y la metadata asociada.
    /// </summary>
    public class Label : SecureViewBase, IHasModelData
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Label"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public Label(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            Encode = true;
        }

        /// <summary>
        /// Obtiene o establece si el texto de la etiqueta debe ser codificado como HTML.
        /// </summary>
        public bool Encode { get; set; }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará dentro de la etiqueta <c>&lt;label&gt;</c>.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Obtiene o establece el valor del atributo <c>for</c> de la etiqueta <c>&lt;label&gt;</c>,
        /// que asocia la etiqueta a un control de formulario.
        /// </summary>
        public string? For { get; set; }

#if !NETCOREAPP && !NETSTANDARD
        /// <summary>
        /// Obtiene o establece la metadata del modelo asociada a la etiqueta (MVC clásico).
        /// </summary>
        public ModelMetadata? Metadata
#else
        /// <summary>
        /// Obtiene o establece el explorador de modelo asociado a la etiqueta (ASP.NET Core).
        /// </summary>
        public ModelExplorer? Metadata
#endif
        {
            get;
            set;
        }

        /// <summary>
        /// Escribe el HTML de la etiqueta <c>&lt;label&gt;</c> en el escritor especificado.
        /// Incluye el texto, atributos, clases CSS y el atributo <c>for</c> si corresponde.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            string innerText = Text ?? string.Empty;
            string? fullName = Name;

            var tagBuilder = new TagBuilder("label");
#if !NETCOREAPP && !NETSTANDARD
            if (Encode)
                tagBuilder.SetInnerText(innerText);
            else
                tagBuilder.InnerHtml = innerText;
#else
            if (Encode)
                tagBuilder.InnerHtml.SetContent(innerText);
            else
                tagBuilder.InnerHtml.AppendHtml(innerText);
#endif
            tagBuilder.MergeAttributes(HtmlAttributes);
            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            // Si hay metadatos, se obtiene la etiqueta asociada al control.
#if !NETCOREAPP && !NETSTANDARD
            ModelMetadata? metadata = this.Metadata;
            string? propertyName = this.Name;
            if (metadata is null && !string.IsNullOrEmpty(this.Name))
                ModelExplorerUtils.FromStringExpression(this.Name!, this.ViewContext.ViewData, out metadata, out propertyName);

            string label = ElementUtils.GetControlLabel(metadata, propertyName);
            tagBuilder.SetInnerText(label);
#else
            ModelExplorer? modelExplorer = this.Metadata;
            string? propertyName = this.Name;
            if (modelExplorer is null && !string.IsNullOrEmpty(this.Name))
                ModelExplorerUtils.FromStringExpression(this.Name!, HtmlHelper, out modelExplorer, out propertyName);

            ModelMetadata? metadata = modelExplorer is not null ? modelExplorer.Metadata : null;
            string label = ElementUtils.GetControlLabel(metadata, propertyName);
            tagBuilder.InnerHtml.Append(label);
#endif

            string? _for = this.For;
            if (string.IsNullOrEmpty(_for))
                _for = propertyName;

            if (!string.IsNullOrEmpty(_for))
                tagBuilder.MergeAttribute("for", _for);

#if !NETCOREAPP && !NETSTANDARD
            writer.Write(tagBuilder.ToString(TagRenderMode.Normal));
#else
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}
