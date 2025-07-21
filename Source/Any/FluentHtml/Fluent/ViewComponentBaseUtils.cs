using System.Collections.Generic;
using FluentHtml.Fluent;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;


#if NETSTANDARD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
#else
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#endif

namespace FluentHtml.Html
{
    /// <summary>
    /// Utilidades auxiliares para componentes basados en <see cref="ViewComponentBase"/>,
    /// que permiten fusionar atributos de validación y clases CSS en los controles HTML generados.
    /// </summary>
    public static class ViewComponentBaseUtils
    {
        private static readonly string[] InputTypesWithRanges = new string[]
        {
            "number",
            "range",
            "date",
            "datetime-local",
            "month",
            "time",
            "week"
        };

        /// <summary>
        /// Fusiona los atributos de validación y las clases CSS correspondientes en el <see cref="TagBuilder"/>
        /// para el componente especificado, utilizando la metadata del modelo.
        /// </summary>
        /// <param name="viewComponentBase">Instancia del componente de vista base.</param>
        /// <param name="tagBuilder">Instancia de <see cref="TagBuilder"/> donde se fusionarán los atributos.</param>
        /// <param name="metadata">Metadata del modelo asociada al campo.</param>
#if !NETCOREAPP && !NETSTANDARD
        public static void MergeValidationAttributes(this ViewComponentBase viewComponentBase, TagBuilder tagBuilder, ModelMetadata? metadata)
        {
            if (viewComponentBase is null)
                throw new ArgumentNullException(nameof(viewComponentBase));

            bool ensureValidations = viewComponentBase.FluentHelper.GetAppSettingValue<bool>(FluentHelper.ENSUREVALIDATIONSWITHVIEWCONTROLS);
            string? fullName = viewComponentBase.Name;
            // If there are any errors for a named field, we add the css attribute.

            ModelState modelState;
            if (viewComponentBase.HtmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);

            string? propertyName = viewComponentBase.Name;
            if (metadata is null && !string.IsNullOrEmpty(fullName))
                ModelExplorerUtils.FromStringExpression(fullName!, viewComponentBase.ViewContext.ViewData, out metadata, out propertyName);

            if (metadata is not null && !string.IsNullOrEmpty(propertyName))
            {
                IDictionary<string, object> validationAttributes = viewComponentBase.HtmlHelper.GetUnobtrusiveValidationAttributes(propertyName!, metadata, false);
                tagBuilder.MergeAttributes(validationAttributes);

                if (ensureValidations)
                {
                    IEnumerable<ModelClientValidationRule> validators = ModelValidatorProviders.Providers.GetValidators(metadata, viewComponentBase.HtmlHelper.ViewContext)
                                                                            .SelectMany(v => v.GetClientValidationRules());

                    foreach (ModelClientValidationRule validator in validators)
                    {
                        if (validator.ValidationType == "length")
                        {
                            if (!tagBuilder.Attributes.ContainsKey("maxlength") && validator.HasValidValidationParameter("max"))
                            {
                                object maxLength = validator.ValidationParameters["max"];
                                tagBuilder.MergeAttribute("maxlength", maxLength.ToString());
                            }
                        }
                        else if (validator.ValidationType == "range")
                        {
                            string? type = tagBuilder.Attributes.TryGetValue("type", out string? _type) && _type is not null ? _type.ToLower() : null;
                            bool applyRangeAttributes = type is not null ? InputTypesWithRanges.Contains(type) : false;
                            if (applyRangeAttributes)
                            {
                                if (!tagBuilder.Attributes.ContainsKey("min") && validator.HasValidValidationParameter("min"))
                                {
                                    object min = validator.ValidationParameters["min"];
                                    tagBuilder.MergeAttribute("min", min.ToString());
                                }

                                if (!tagBuilder.Attributes.ContainsKey("max") && validator.HasValidValidationParameter("max"))
                                {
                                    object max = validator.ValidationParameters["max"];
                                    tagBuilder.MergeAttribute("max", max.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }
#else
        public static void MergeValidationAttributes(this ViewComponentBase viewComponentBase, TagBuilder tagBuilder, ModelExplorer? metadata)
        {
            if (viewComponentBase is null)
                throw new ArgumentNullException(nameof(viewComponentBase));

            bool ensureValidations = viewComponentBase.FluentHelper.GetAppSettingValue<bool>(FluentHelper.ENSUREVALIDATIONSWITHVIEWCONTROLS);
            string? fullName = viewComponentBase.Name;
            // If there are any errors for a named field, we add the css attribute.

            if (!string.IsNullOrEmpty(fullName) && viewComponentBase.HtmlHelper.ViewData.ModelState.TryGetValue(fullName, out ModelStateEntry? modelState) && modelState is not null && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper.ValidationInputCssClassName);

            ModelExplorer? modelExplorer = metadata;
            string? propertyName = viewComponentBase.Name;
            if (modelExplorer is null && !string.IsNullOrEmpty(fullName))
                ModelExplorerUtils.FromStringExpression(fullName!, viewComponentBase.HtmlHelper, out modelExplorer, out propertyName);

            ValidationHtmlAttributeProvider? validationHtmlAttributeProvider = (ValidationHtmlAttributeProvider?)viewComponentBase.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ValidationHtmlAttributeProvider));
            if (validationHtmlAttributeProvider is not null)
            {
                Dictionary<string, string?> validationAttributes = new Dictionary<string, string?>();
                validationHtmlAttributeProvider.AddValidationAttributes(viewComponentBase.HtmlHelper.ViewContext, modelExplorer, validationAttributes);
                tagBuilder.MergeAttributes(validationAttributes);
            }

            if (ensureValidations)
            {
                var clientValidatorCache = new ClientValidatorCache();
                IOptions<MvcViewOptions>? optionsAccessor = (IOptions<MvcViewOptions>?)viewComponentBase.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IOptions<MvcViewOptions>));
                if (optionsAccessor is null)
                    throw new System.Exception(string.Format(Resources.ServiceNotAdded, typeof(IOptions<MvcViewOptions>)));

                var clientValidatorProviders = optionsAccessor.Value.ClientModelValidatorProviders;
                var clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);

                if (modelExplorer is not null)
                {
                    var validators = clientValidatorCache.GetValidators(modelExplorer.Metadata, clientModelValidatorProvider);

                    foreach (var validator in validators)
                    {
                        if (validator is AttributeAdapterBase<StringLengthAttribute> stringLengthAttributeAdapter)
                        {
                            if (!tagBuilder.Attributes.ContainsKey("maxlength"))
                                tagBuilder.MergeAttribute("maxlength", stringLengthAttributeAdapter.Attribute.MaximumLength.ToString());
                        }
                        else if (validator is AttributeAdapterBase<RangeAttribute> rangeAttributeAdapter)
                        {
                            string? type = tagBuilder.Attributes.TryGetValue("type", out string? _type) && _type is not null ? _type.ToLower() : null;
                            bool applyRangeAttributes = type is not null ? InputTypesWithRanges.Contains(type) : false;

                            if (applyRangeAttributes)
                            {
                                if (!tagBuilder.Attributes.ContainsKey("min") && rangeAttributeAdapter.Attribute.HasValidMinimumValue())
                                    tagBuilder.MergeAttribute("min", rangeAttributeAdapter.Attribute.Minimum.ToString());

                                if (!tagBuilder.Attributes.ContainsKey("max") && rangeAttributeAdapter.Attribute.HasValidMaximumValue())
                                    tagBuilder.MergeAttribute("max", rangeAttributeAdapter.Attribute.Maximum.ToString());
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}
