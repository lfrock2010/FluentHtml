using System;
using System.Collections.Generic;
using System.Globalization;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Linq;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>  
    /// Base class for all fluent components.  
    /// </summary>  
    public abstract class ComponentBase
    {
        /// <summary>  
        /// Gets the <see cref="FluentHelper"/> instance associated with this component.  
        /// </summary>  
        public FluentHelper FluentHelper
        {
            get;
            private set;
        }

        /// <summary>  
        /// Gets the <see cref="IHtmlHelper"/> instance used for rendering HTML.  
        /// </summary>  
        public IHtmlHelper HtmlHelper
        {
            get;
            private set;
        }

        /// <summary>  
        /// Gets the HTML attributes for the component.  
        /// </summary>  
        public IDictionary<string, object?> HtmlAttributes
        {
            get;
            private set;
        }

        /// <summary>  
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.  
        /// </summary>  
        /// <param name="fluentHelper">The <see cref="FluentHelper"/> instance to associate with this component.</param>  
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fluentHelper"/> is null.</exception>  
        protected ComponentBase(FluentHelper fluentHelper)
        {
            if (fluentHelper is null)
                throw new ArgumentNullException(nameof(fluentHelper));

            this.FluentHelper = fluentHelper;
            this.HtmlHelper = fluentHelper.HtmlHelper;
            this.HtmlAttributes = new RouteValueDictionary();
        }

        /// <summary>  
        /// Formats the given value into a string using the specified format.  
        /// </summary>  
        /// <param name="value">The value to format. Can be null.</param>  
        /// <param name="format">The format string to apply. If null or empty, the default format is used.</param>  
        /// <returns>A formatted string representation of the value.</returns>  
        protected string FormatValue(object? value, string? format)
        {
            // From ViewDataDictionary.FormatValueInternal(), which is called from HtmlHelper.FormatValue()  
            // Reproduced here to remove dependency on ASP.NET MVC 4  
            if (value is null)
                return string.Empty;

            Type valueType = value.GetType();
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
                valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            if (valueType.IsEnum)
            {
                valueType = Enum.GetUnderlyingType(valueType);
                value = Convert.ChangeType(value, valueType);
            }
            else if (valueType == typeof(byte[]) && value is byte[] inArray)
            {
                value = Convert.ToBase64String(inArray);
            }
            else if (valueType == typeof(DateTime))
            {
                value = ((DateTime?)value).GetValueOrDefault();
                if (string.IsNullOrEmpty(format))
                    format = "{0:s}";
            }
#if !NETCOREAPP && !NETSTANDARD
            else if (typeof(Binary).IsAssignableFrom(valueType) && value is Binary binary)
            {
                value = binary.ToArray();
            }
#endif

            if (string.IsNullOrEmpty(format))
                return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;

            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            object[] objArray = new object[] { value };
            return string.Format(currentCulture, format, objArray);
        }
    }
}
