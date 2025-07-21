using FluentHtml.Extensions;
using FluentHtml.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Text.Encodings.Web;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
#else
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#endif

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Representa un control de casilla de verificación (checkbox) HTML configurable, con soporte para atributos personalizados, validación y control de permisos.
    /// </summary>
    public class CheckBox : InputBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CheckBox"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public CheckBox(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            InputType = InputType.Checkbox;
            IncludeHidden = true;
        }

        /// <summary>
        /// Indica si la casilla debe aparecer marcada.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Indica si se debe renderizar un campo oculto adicional para el valor "false".
        /// </summary>
        public bool IncludeHidden { get; set; }

        /// <summary>
        /// Escribe el HTML de la casilla de verificación (y el campo oculto si corresponde) en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            VerifySettings();
            var inputHtml = GetCheckboxHtml();
#if !NETCOREAPP && !NETSTANDARD
                writer.WriteLine(inputHtml.ToString(TagRenderMode.SelfClosing));
#else
            inputHtml.WriteTo(writer, HtmlEncoder.Default);
#endif

            if (!IncludeHidden)
                return;

            // Renderiza un <input type="hidden".../> adicional para checkboxes.
            var hiddenHtml = GetHiddenHtml();
#if !NETCOREAPP && !NETSTANDARD
                writer.Write(hiddenHtml.ToString(TagRenderMode.SelfClosing));
#else
            hiddenHtml.WriteTo(writer, HtmlEncoder.Default);
#endif
        }

        /// <summary>
        /// Genera el HTML para el campo oculto que representa el valor "false" de la casilla.
        /// </summary>
        /// <returns>Instancia de <see cref="TagBuilder"/> para el campo oculto.</returns>
        protected TagBuilder GetHiddenHtml()
        {
            var hiddenInput = new TagBuilder("input");
            hiddenInput.MergeAttribute("type", "hidden");
            hiddenInput.MergeAttribute("name", Name);
            hiddenInput.MergeAttribute("value", "false");
            return hiddenInput;
        }

        /// <summary>
        /// Genera el HTML para el control de casilla de verificación.
        /// </summary>
        /// <returns>Instancia de <see cref="TagBuilder"/> para el checkbox.</returns>
        protected TagBuilder GetCheckboxHtml()
        {
            string? fullName = Name;
            var inputItem = EnumItem<InputType>.Create(InputType);
            string? inputType = inputItem?.Description.ToLowerInvariant();

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(HtmlAttributes);
            tagBuilder.MergeAttribute("type", inputType);

            tagBuilder.MergeAttribute("name", fullName, true);
            if (!this.HtmlAttributes.ContainsKey("id") && !string.IsNullOrEmpty(fullName))
#if !NETCOREAPP && !NETSTANDARD
                    tagBuilder.GenerateId(fullName);
#else
                tagBuilder.GenerateId(fullName, this.HtmlHelper.IdAttributeDotReplacement);
#endif

            tagBuilder.MergeAttribute("value", "true", true);

            if (Checked)
                tagBuilder.MergeAttribute("checked", "checked");

            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            // Si hay errores de validación para el campo, se agregan los atributos de validación.
            this.MergeValidationAttributes(tagBuilder, this.Metadata);

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

            return tagBuilder;
        }
    }
}