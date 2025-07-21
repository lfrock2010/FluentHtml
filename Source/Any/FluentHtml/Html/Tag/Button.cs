using System;
using System.IO;
using FluentHtml.Extensions;
using FluentHtml.Fluent;
using FluentHtml.Reflection;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
#endif

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Representa un botón HTML configurable y seguro, que permite personalizar el tipo, el texto, el comportamiento de codificación y las clases CSS según los permisos de acceso.
    /// </summary>
    public class Button : SecureViewBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Button"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public Button(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            Encode = true;
        }

        /// <summary>
        /// Indica si el texto del botón debe ser codificado para HTML.
        /// </summary>
        public bool Encode { get; set; }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará dentro del botón.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de botón (button, submit, reset).
        /// </summary>
        public ButtonType ButtonType { get; set; }

        /// <summary>
        /// Escribe el HTML del botón en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            var inputItem = EnumItem<ButtonType>.Create(ButtonType);
            string? inputType = inputItem?.Description?.ToLowerInvariant();

            string fullName = Name ?? string.Empty;
            string innerText = Text ?? string.Empty;

            var tagBuilder = new TagBuilder("button");
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
            tagBuilder.MergeAttribute("type", inputType);

            if (fullName.HasValue())
            {
                tagBuilder.MergeAttribute("name", fullName, true);
                if (!this.HtmlAttributes.ContainsKey("id"))
#if !NETCOREAPP && !NETSTANDARD
                    tagBuilder.GenerateId(fullName);
#else
                    tagBuilder.GenerateId(fullName, this.HtmlHelper.IdAttributeDotReplacement);
#endif
            }

            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            if (!CanWrite())
            {
                tagBuilder.MergeAttribute("disabled", "disabled", true);
                if (DeniedClass.HasValue())
                    tagBuilder.AddCssClass(DeniedClass);
            }
            else if (GrantedClass.HasValue())
            {
                tagBuilder.AddCssClass(GrantedClass!);
            }

#if !NETCOREAPP && !NETSTANDARD
            writer.Write(tagBuilder.ToString(TagRenderMode.Normal));
#else
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}
