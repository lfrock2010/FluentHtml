using System.Collections.Generic;
using System.Reflection;

namespace System.Web.Mvc
{
    internal static class HtmlHelperExtensions
    {
        public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(this HtmlHelper htmlHelper, string name, bool clearData)
        {
            return HtmlHelperExtensions.GetUnobtrusiveValidationAttributes(htmlHelper, name, null, clearData);
        }

        public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(this HtmlHelper htmlHelper, string name, ModelMetadata metadata, bool clearData)
        {
            IDictionary<string, object> results = new Dictionary<string, object>();
            if (!clearData)
            {
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    FormContext formContextForClientValidation = htmlHelper.ViewContext.GetFormContextForClientValidation();
                    if (formContextForClientValidation == null)
                        return results;

                    string fullHtmlFieldName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
                    Func<string, ModelMetadata, IEnumerable<ModelClientValidationRule>> clientValidationRuleFactory = htmlHelper.GetClientValidationRuleFactory();
                    UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(clientValidationRuleFactory(name, metadata), results);
                }
            }
            else
                results = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);

            return results;
        }

        internal static Func<string, ModelMetadata, IEnumerable<ModelClientValidationRule>> GetClientValidationRuleFactory(this HtmlHelper htmlHelper)
        {
            if (htmlHelper == null)
                return null;

            PropertyInfo property = typeof(HtmlHelper).GetProperty("ClientValidationRuleFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            return property != null ? (Func<string, ModelMetadata, IEnumerable<ModelClientValidationRule>>)property.GetValue(htmlHelper, null) : null;
        }
    }
}
