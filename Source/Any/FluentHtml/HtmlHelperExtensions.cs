using System.Collections.Generic;
using System.Reflection;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
using RequestContext = Microsoft.AspNetCore.Mvc.Rendering.FluentRequestContext;
#endif

namespace System.Web.Mvc
{
    internal static class HtmlHelperExtensions
    {
#if !NETCOREAPP && !NETSTANDARD
        public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(this IHtmlHelper htmlHelper, string name, bool clearData)
        {
            return HtmlHelperExtensions.GetUnobtrusiveValidationAttributes(htmlHelper, name, null, clearData);
        }

        public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(this IHtmlHelper htmlHelper, string name, ModelMetadata? metadata, bool clearData)
        {
            IDictionary<string, object> results = new Dictionary<string, object>();
            if (!clearData)
            {
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    FormContext? formContextForClientValidation = htmlHelper.ViewContext.GetFormContextForClientValidation();
                    if (formContextForClientValidation is null)
                        return results;

                    string fullHtmlFieldName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
                    Func<string, ModelMetadata?, IEnumerable<ModelClientValidationRule>>? clientValidationRuleFactory = htmlHelper.GetClientValidationRuleFactory();
                    if (clientValidationRuleFactory is not null)
                        UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(clientValidationRuleFactory(name, metadata), results);
                }
            }
            else
                results = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);

            return results;
        }

        internal static Func<string, ModelMetadata?, IEnumerable<ModelClientValidationRule>>? GetClientValidationRuleFactory(this HtmlHelper htmlHelper)
        {
            if (htmlHelper is null)
                return null;

            PropertyInfo property = typeof(HtmlHelper).GetProperty("ClientValidationRuleFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            return property is not null ? (Func<string, ModelMetadata?, IEnumerable<ModelClientValidationRule>>)property.GetValue(htmlHelper, null) : null;
        }
#endif
    }
}
