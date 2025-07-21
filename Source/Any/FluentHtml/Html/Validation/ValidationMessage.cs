using FluentHtml.Extensions;
using FluentHtml.Fluent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using _HtmlHelper = System.Web.Mvc.HtmlHelper;
using ModelStateEntry = System.Web.Mvc.ModelState;
#else
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Encodings.Web;
using _HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
#endif

namespace FluentHtml.Html.Validation
{
    /// <summary>
    /// Representa un componente de mensaje de validación para mostrar errores de validación asociados a un modelo o campo específico.
    /// </summary>
    public class ValidationMessage : ViewComponentBase, IHasModelData
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationMessage"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public ValidationMessage(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            this.ValidationInputValidCssClassName = _HtmlHelper.ValidationInputValidCssClassName;
            this.ValidationErrorCssClassName = _HtmlHelper.ValidationMessageCssClassName;
            this.ValidationValidCssClassName = _HtmlHelper.ValidationMessageValidCssClassName;
        }

#if !NETCOREAPP && !NETSTANDARD
        /// <summary>
        /// Obtiene o establece la metadata del modelo asociada al mensaje de validación.
        /// </summary>
        public ModelMetadata? Metadata { get; set; }
#else
        /// <summary>
        /// Obtiene o establece el explorador de modelo asociado al mensaje de validación.
        /// </summary>
        public ModelExplorer? Metadata { get; set; }
#endif

        /// <summary>
        /// Obtiene o establece el nombre del modelo o campo al que está asociado el mensaje de validación.
        /// </summary>
        public string? ModelName { get; set; }

        /// <summary>
        /// Obtiene o establece el mensaje de validación personalizado a mostrar.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la clase CSS para un input válido.
        /// </summary>
        public string ValidationInputValidCssClassName { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la clase CSS para un mensaje de error de validación.
        /// </summary>
        public string ValidationErrorCssClassName { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la clase CSS para un mensaje de validación válido.
        /// </summary>
        public string ValidationValidCssClassName { get; set; }

        /// <summary>
        /// Indica si se debe mostrar el mensaje como atributo <c>title</c> en el HTML.
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Indica si solo se debe mostrar el icono de validación, sin el texto del mensaje.
        /// </summary>
        public bool IconOnly { get; set; }

        /// <summary>
        /// Escribe el HTML del mensaje de validación en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            string modelName = this.ModelName ?? string.Empty;
            FormContext? formContext = GetFormContextForClientValidation(ViewContext);

            if (string.IsNullOrEmpty(modelName) || !HtmlHelper.ViewData.ModelState.ContainsKey(modelName) && formContext is null)
                return;

            var modelState = HtmlHelper.ViewData.ModelState[modelName];

            var modelErrorCollection = modelState is null ? null : modelState.Errors;
            var error = (modelErrorCollection is null || modelErrorCollection.Count == 0)
                ? null : modelErrorCollection.FirstOrDefault(m => !string.IsNullOrEmpty(m.ErrorMessage)) ?? modelErrorCollection[0];

            if (error is null && formContext is null)
                return;

            var tagBuilder = new TagBuilder("span");
            tagBuilder.MergeAttributes(HtmlAttributes);
            tagBuilder.AddCssClass(this.ValidationInputValidCssClassName);
            tagBuilder.AddCssClass(error is not null ? ValidationErrorCssClassName : ValidationValidCssClassName);

            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            string? message = Message ?? string.Empty;
            if (string.IsNullOrEmpty(message) && error is not null)
                message = GetUserErrorMessageOrDefault(HtmlHelper, error, modelState) ?? string.Empty;

            if (message.HasValue())
            {
                if (this.ShowTitle)
                    tagBuilder.MergeAttribute("title", message, true);

#if !NETCOREAPP && !NETSTANDARD
                if (!IconOnly)
                    tagBuilder.SetInnerText(message);
#else
                if (!IconOnly)
                    tagBuilder.InnerHtml.SetContent(message);
#endif
            }

            if (formContext is null)
            {
#if !NETCOREAPP && !NETSTANDARD
                writer.Write(tagBuilder.ToString(TagRenderMode.Normal));
#else
                tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
                return;
            }

            bool replaceValidationMessageContents = string.IsNullOrEmpty(Message);
#if !NETCOREAPP && !NETSTANDARD
            if (ViewContext.UnobtrusiveJavaScriptEnabled)
            {
                tagBuilder.MergeAttribute("data-valmsg-for", modelName);
                tagBuilder.MergeAttribute("data-valmsg-replace", replaceValidationMessageContents.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
            }
            else
            {
                if (this.Metadata is not null && !string.IsNullOrEmpty(modelName))
                {
                    FieldValidationMetadata validationMetadata = ApplyFieldValidationMetadata(HtmlHelper, Metadata, modelName);
                    // rules will already have been written to the metadata object
                    validationMetadata.ReplaceValidationMessageContents = replaceValidationMessageContents;

                    // client validation always requires an ID
                    string id = modelName;
                    if (this.HtmlAttributes.ContainsKey("id"))
                        id = this.HtmlAttributes["id"]?.ToString() ?? string.Empty;

                    tagBuilder.GenerateId(id + "_validationMessage");
                    validationMetadata.ValidationMessageId = tagBuilder.Attributes["id"];
                }
            }

            writer.Write(tagBuilder.ToString(TagRenderMode.Normal));
#else
            tagBuilder.MergeAttribute("data-valmsg-for", modelName);
            tagBuilder.MergeAttribute("data-valmsg-replace", replaceValidationMessageContents.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }

        /// <summary>
        /// Obtiene el contexto de formulario para la validación del cliente, si está habilitada.
        /// </summary>
        /// <param name="viewContext">El contexto de la vista.</param>
        /// <returns>Instancia de <see cref="FormContext"/> si la validación del cliente está habilitada; de lo contrario, <c>null</c>.</returns>
        private static FormContext? GetFormContextForClientValidation(ViewContext viewContext)
        {
            return !viewContext.ClientValidationEnabled ? null : viewContext.FormContext;
        }

#if !NETCOREAPP && !NETSTANDARD
            /// <summary>
            /// Aplica la metadata de validación de campo para la validación del cliente.
            /// </summary>
            /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper"/>.</param>
            /// <param name="modelMetadata">Metadata del modelo.</param>
            /// <param name="modelName">Nombre del modelo o campo.</param>
            /// <returns>Instancia de <see cref="FieldValidationMetadata"/> con las reglas de validación aplicadas.</returns>
            private static FieldValidationMetadata ApplyFieldValidationMetadata(IHtmlHelper htmlHelper, ModelMetadata modelMetadata, string modelName)
            {            
                FieldValidationMetadata metadataForField = htmlHelper.ViewContext.FormContext.GetValidationMetadataForField(modelName, true);
                var modelClientValidationRules = ModelValidatorProviders.Providers.GetValidators(modelMetadata, htmlHelper.ViewContext).SelectMany(v => v.GetClientValidationRules());
                foreach (ModelClientValidationRule clientValidationRule in modelClientValidationRules)
                    metadataForField.ValidationRules.Add(clientValidationRule);

                return metadataForField;
            }
#endif

        /// <summary>
        /// Obtiene el mensaje de recurso predeterminado para valores de propiedad inválidos.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <returns>Mensaje de error predeterminado.</returns>
        private static string GetInvalidPropertyValueResource(IHtmlHelper htmlHelper)
        {
#if !NETCOREAPP && !NETSTANDARD
                string? message = null;            

                if (!string.IsNullOrEmpty(System.Web.Mvc.Html.ValidationExtensions.ResourceClassKey) && htmlHelper is not null)
                    message = htmlHelper.ViewContext.HttpContext.GetGlobalResourceObject(System.Web.Mvc.Html.ValidationExtensions.ResourceClassKey, "InvalidPropertyValue", CultureInfo.CurrentUICulture) as string;                       

                return message ?? "The value '{0}' is invalid.";
#else
            return "The value '{0}' is invalid.";
#endif
        }

        /// <summary>
        /// Obtiene el mensaje de error definido por el usuario o el mensaje predeterminado si no existe.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <param name="error">Instancia de <see cref="ModelError"/>.</param>
        /// <param name="modelState">Estado del modelo asociado.</param>
        /// <returns>Mensaje de error a mostrar.</returns>
        private static string? GetUserErrorMessageOrDefault(IHtmlHelper htmlHelper, ModelError error, ModelStateEntry? modelState)
        {
            if (!string.IsNullOrEmpty(error.ErrorMessage))
                return error.ErrorMessage;

            if (modelState is null)
                return null;

#if !NETCOREAPP && !NETSTANDARD
                string? str = modelState.Value is not null ? modelState.Value.AttemptedValue : null;
#else
            string? str = modelState.AttemptedValue;
#endif
            return string.Format(CultureInfo.CurrentCulture, GetInvalidPropertyValueResource(htmlHelper), new object?[] { str });
        }
    }
}