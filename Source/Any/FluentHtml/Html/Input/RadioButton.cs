using System;
using System.Collections.Generic;
using System.IO;
using FluentHtml.Extensions;
using FluentHtml.Reflection;

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
    /// Representa un control de botón de opción (<c>&lt;input type="radio"&gt;</c>) seguro y configurable,
    /// permitiendo personalizar el valor, el estado seleccionado, los atributos HTML y la integración con metadatos de modelo.
    /// </summary>
    public class RadioButton : InputBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RadioButton"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public RadioButton(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            InputType = InputType.Radio;
        }

        /// <summary>
        /// Obtiene o establece si el botón de opción está seleccionado (<c>checked</c>).
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Escribe el HTML del botón de opción en el escritor especificado.
        /// Incluye atributos, clases CSS, validación y control de permisos.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            VerifySettings();

            string? fullName = Name;

            var inputItem = EnumItem<InputType>.Create(InputType);
            string? inputType = inputItem?.Description?.ToLowerInvariant();

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

            object value = Value ?? string.Empty;
            string valueParameter = this.FormatValue(value, Format);

            tagBuilder.MergeAttribute("value", valueParameter, true);

            // Si está marcado explícitamente o el valor del modelo coincide, se marca como seleccionado.
            if (Checked || (Metadata is not null && Metadata.Model == Value))
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

#if !NETCOREAPP && !NETSTANDARD
                writer.Write(tagBuilder.ToString(TagRenderMode.SelfClosing));
#else
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}
