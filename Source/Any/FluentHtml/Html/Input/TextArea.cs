using System;
using System.Collections.Generic;
using System.IO;
using FluentHtml.Extensions;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
#else
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#endif

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Representa un control HTML <c>&lt;textarea&gt;</c> para la edición de texto multilínea.
    /// Permite aplicar atributos personalizados, validaciones y control de acceso por roles.
    /// </summary>
    public class TextArea : InputBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TextArea"/> con el helper fluido especificado.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public TextArea(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            InputType = InputType.Text;
        }

        /// <summary>
        /// Escribe la representación HTML del control <c>&lt;textarea&gt;</c> en el escritor especificado.
        /// Aplica atributos, clases CSS, validaciones y control de acceso según la configuración y el contexto.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribe el HTML generado.</param>
        public override void WriteTo(TextWriter writer)
        {
            VerifySettings();

            string? fullName = Name;

            // Crea el tag builder para el elemento textarea y aplica atributos personalizados.
            var tagBuilder = new TagBuilder("textarea");
            MergeAttributes(tagBuilder);

            // Asigna el atributo name y genera el id si es necesario.
            tagBuilder.MergeAttribute("name", fullName, true);
            if (!this.HtmlAttributes.ContainsKey("id") && !string.IsNullOrEmpty(fullName))
#if !NETCOREAPP && !NETSTANDARD
                    tagBuilder.GenerateId(fullName);
#else
                tagBuilder.GenerateId(fullName, this.HtmlHelper.IdAttributeDotReplacement);
#endif

            // Obtiene y formatea el valor a mostrar en el textarea.
            object value = Value ?? string.Empty;
            string valueParameter = this.FormatValue(value, Format);

#if !NETCOREAPP && !NETSTANDARD
                tagBuilder.SetInnerText(valueParameter);
#else
            tagBuilder.InnerHtml.SetContent(valueParameter);
#endif

            // Agrega las clases CSS definidas.
            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            // Si existen errores de validación para el campo, se agregan los atributos correspondientes.
            this.MergeValidationAttributes(tagBuilder, this.Metadata);

            // Control de acceso: si no puede escribir, se marca como solo lectura y se aplica la clase denegada.
            if (!CanWrite())
            {
                tagBuilder.MergeAttribute("readonly", "readonly", true);
                if (DeniedClass.HasValue())
                    tagBuilder.AddCssClass(DeniedClass);
            }
            // Si el acceso es concedido, se agrega la clase CSS correspondiente.
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
