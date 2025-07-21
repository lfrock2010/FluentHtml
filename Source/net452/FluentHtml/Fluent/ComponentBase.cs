using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Routing;

namespace FluentHtml.Fluent
{
    public abstract class ComponentBase
    {
        protected ComponentBase()
        {
            HtmlAttributes = new RouteValueDictionary();
        }

        public IDictionary<string, object> HtmlAttributes { get; private set; }

        protected string FormatValue(object value, string format)
        {
            // From ViewDataDictionary.FormatValueInternal(), which is called from HtmlHelper.FormatValue()
            // Reproduced here to remove dependency on ASP.NET MVC 4
            if (value == null)
                return string.Empty;

            if (value is DateTime || value is DateTime?)
            {
                value = ((DateTime?)value).GetValueOrDefault();
                if (String.IsNullOrEmpty(format))
                    format = "{0:s}";
            }

            if (string.IsNullOrEmpty(format))
            {
                return Convert.ToString(value, CultureInfo.CurrentCulture);
            }
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            object[] objArray = new object[] { value };
            return string.Format(currentCulture, format, objArray);
        }

    }
}
