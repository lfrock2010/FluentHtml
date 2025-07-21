using System;
using FluentHtml.Extensions;
using FluentHtml.Fluent;
using System.IO;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
#endif

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Representa un enlace HTML <c>&lt;a&gt;</c> configurable y seguro,
    /// permitiendo personalizar el texto, la navegación, la codificación y los atributos de seguridad.
    /// </summary>
    public class Link : SecureViewBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Link"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public Link(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            Encode = true;
            Navigation = new NavigationRequest();
        }

        /// <summary>
        /// Obtiene o establece si el texto del enlace debe ser codificado como HTML.
        /// </summary>
        public bool Encode { get; set; }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará dentro del enlace.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Obtiene o establece la información de navegación (ruta, acción, controlador, url) para el enlace.
        /// </summary>
        public NavigationRequest Navigation { get; set; }

        /// <summary>
        /// Escribe el HTML del enlace <c>&lt;a&gt;</c> en el escritor especificado.
        /// Incluye el texto, atributos, clases CSS, href y control de permisos.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            string href = UrlGenerator.Generate(HtmlHelper, Navigation);
            string innerText = Text ?? string.Empty;
            string? fullName = Name;

            var tagBuilder = new TagBuilder("a");
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

            if (!this.HtmlAttributes.ContainsKey("id") && !string.IsNullOrEmpty(fullName))
#if !NETCOREAPP && !NETSTANDARD
                    tagBuilder.GenerateId(fullName);
#else
                tagBuilder.GenerateId(fullName, this.HtmlHelper.IdAttributeDotReplacement);
#endif

            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            if (CanWrite())
            {
                if (href.HasValue())
                    tagBuilder.MergeAttribute("href", href);

                if (GrantedClass.HasValue())
                    tagBuilder.AddCssClass(GrantedClass!);
            }
            else
            {
                tagBuilder.MergeAttribute("disabled", "disabled", true);
                if (DeniedClass.HasValue())
                    tagBuilder.AddCssClass(DeniedClass!);
            }

#if !NETCOREAPP && !NETSTANDARD
                writer.Write(tagBuilder.ToString(TagRenderMode.Normal));
#else
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}
