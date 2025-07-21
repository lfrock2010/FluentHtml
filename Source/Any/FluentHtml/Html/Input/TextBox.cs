using FluentHtml.Extensions;
using FluentHtml.Reflection;
using System.Collections.Generic;
using System.IO;

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
    /// Representa un control de entrada HTML de tipo <c>input type="text"</c> para la edición de texto de una sola línea.
    /// Permite aplicar validaciones, atributos personalizados y control de acceso por roles.
    /// </summary>
    public class TextBox : InputBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TextBox"/> con el helper fluido especificado.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public TextBox(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            InputType = InputType.Text;
        }

        /// <summary>
        /// Escribe la representación HTML del control <c>input type="text"</c> en el escritor especificado.
        /// Aplica atributos, clases CSS, validaciones y control de acceso según la configuración y el contexto.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribe el HTML generado.</param>
        public override void WriteTo(TextWriter writer)
        {
            VerifySettings();

            string? fullName = Name;

            // Obtiene la descripción del tipo de input (por ejemplo, "text", "password").
            var inputItem = EnumItem<InputType>.Create(InputType);
            string? inputType = inputItem?.Description?.ToLowerInvariant();

            var tagBuilder = new TagBuilder("input");
            MergeAttributes(tagBuilder);
            tagBuilder.MergeAttribute("type", inputType);

            tagBuilder.MergeAttribute("name", fullName, true);
            // Genera el atributo id si no está presente y el nombre es válido.
            if (!this.HtmlAttributes.ContainsKey("id") && !string.IsNullOrEmpty(fullName))
#if !NETCOREAPP && !NETSTANDARD
                    tagBuilder.GenerateId(fullName);
#else
                tagBuilder.GenerateId(fullName, this.HtmlHelper.IdAttributeDotReplacement);
#endif

            object value = Value ?? string.Empty;
            // Formatea el valor según el formato especificado.
            string valueParameter = this.FormatValue(value, Format);

            tagBuilder.MergeAttribute("value", valueParameter, true);

            // Agrega las clases CSS definidas.
            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            // Si existen errores de validación para el campo, se agregan los atributos correspondientes.
            this.MergeValidationAttributes(tagBuilder, this.Metadata);

            bool canRead = CanRead();
            bool canWrite = CanWrite();

            // Si no tiene permiso de lectura, se muestra como campo de contraseña.
            if (!canRead)
                tagBuilder.MergeAttribute("type", "password", true);

            // Si no tiene permiso de escritura, se marca como solo lectura.
            if (!canWrite)
                tagBuilder.MergeAttribute("readonly", "readonly", true);

            // Si el acceso es denegado, se agrega la clase CSS correspondiente.
            if ((!canRead || !canWrite) && DeniedClass.HasValue())
                tagBuilder.AddCssClass(DeniedClass);

            // Si el acceso es concedido, se agrega la clase CSS correspondiente.
            if (canRead && canWrite && GrantedClass.HasValue())
                tagBuilder.AddCssClass(GrantedClass!);

#if !NETCOREAPP && !NETSTANDARD
                writer.Write(tagBuilder.ToString(TagRenderMode.SelfClosing));
#else
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }

    }
}
