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
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Representa un elemento HTML genérico configurable, permitiendo personalizar la etiqueta, el texto, el modo de codificación y las clases CSS según los permisos de acceso.
    /// </summary>
    public class Element : SecureViewBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Element"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public Element(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            Encode = true;
        }

        /// <summary>
        /// Obtiene o establece el nombre de la etiqueta HTML (por ejemplo, "span", "div", "label").
        /// </summary>
        public string? TagName { get; set; }

        /// <summary>
        /// Indica si el texto del elemento debe ser codificado para HTML.
        /// </summary>
        public bool Encode { get; set; }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará dentro del elemento.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Escribe el HTML del elemento en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            string innerText = Text ?? string.Empty;
            string? fullName = Name;
            string tagName = TagName.HasValue() ? TagName!.ToLower() : "span";

            var tagBuilder = new TagBuilder(tagName);
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
                if (GrantedClass.HasValue())
                    tagBuilder.AddCssClass(GrantedClass!);
            }
            else
            {
                if (DeniedClass.HasValue())
                    tagBuilder.AddCssClass(DeniedClass);
            }
#if !NETCOREAPP && !NETSTANDARD
                writer.Write(tagBuilder.ToString(TagRenderMode.Normal));
#else
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}
