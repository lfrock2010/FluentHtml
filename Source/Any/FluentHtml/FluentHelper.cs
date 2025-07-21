using System;
using System.ComponentModel;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
using System.Configuration;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
#endif

namespace FluentHtml
{
    /// <summary>
    /// Proporciona utilidades y acceso a configuraciones para la generación fluida de HTML en vistas MVC o Razor Pages.
    /// Permite obtener valores de configuración y expone el helper HTML asociado al contexto de la vista.
    /// </summary>
    public class FluentHelper
    {
        internal const string PREFIX = "FluentHtml";
        /// <summary>
        /// Clave de configuración para habilitar la validación en controles de vista.
        /// </summary>
        public const string ENSUREVALIDATIONSWITHVIEWCONTROLS = "EnsureValidationsWithViewControls";
        /// <summary>
        /// Clave de configuración para habilitar el escape de contenido en controles de vista.
        /// </summary>
        public const string ENCODEVIEWCONTROLSCONTENT = "EncodeViewControlsContent";

        /// <summary>
        /// Obtiene o establece el helper HTML asociado al contexto de la vista.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IHtmlHelper HtmlHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FluentHelper"/> con el helper HTML especificado.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper"/> asociada al contexto de la vista.</param>
        public FluentHelper(IHtmlHelper htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }

        /// <summary>
        /// Obtiene el valor de configuración de la aplicación para la clave especificada y lo convierte al tipo indicado.
        /// </summary>
        /// <typeparam name="T">Tipo de valor a devolver.</typeparam>
        /// <param name="index">Clave de configuración.</param>
        /// <returns>Valor convertido al tipo <typeparamref name="T"/> o valor por defecto si no se encuentra.</returns>
        public T? GetAppSettingValue<T>(string index)
        {
#if NETCOREAPP || NETSTANDARD
            T? value = default(T);
            bool success = this.TryGetJsonAppSettingValue<T>(index, out value);

            //if (!success)            
            //    this.TryGetXmlAppSettingValue<T>(index, out value);
            return value;
#else
            T? value = default(T);
            this.TryGetXmlAppSettingValue<T>(index, out value);
            return value;
#endif
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// Intenta obtener el valor de configuración desde un archivo JSON (IConfiguration) y convertirlo al tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de valor a devolver.</typeparam>
        /// <param name="index">Clave de configuración.</param>
        /// <param name="value">Valor convertido si la operación es exitosa; de lo contrario, valor por defecto.</param>
        /// <returns><c>true</c> si se obtiene y convierte el valor correctamente; en caso contrario, <c>false</c>.</returns>
        private bool TryGetJsonAppSettingValue<T>(string index, out T? value)
        {
            value = default(T);
            if (string.IsNullOrEmpty(index) || string.IsNullOrWhiteSpace(index))
                return false;

            IConfiguration? configuration = (IConfiguration?)this.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IConfiguration));
            if (configuration is null)
                return false;

            IConfigurationSection section = configuration.GetSection($"fluentHtml:{index}");
            if (string.IsNullOrEmpty(section.Value))
                return false;

            bool success = false;
            try
            {
                value = (T)Convert.ChangeType(section.Value, typeof(T));
                success = true;
            }
            catch
            {
                success = false;
            }
            return success;
        }
#else
        /// <summary>
        /// Intenta obtener el valor de configuración desde el archivo XML de configuración (AppSettings) y convertirlo al tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de valor a devolver.</typeparam>
        /// <param name="index">Clave de configuración.</param>
        /// <param name="value">Valor convertido si la operación es exitosa; de lo contrario, valor por defecto.</param>
        /// <returns><c>true</c> si se obtiene y convierte el valor correctamente; en caso contrario, <c>false</c>.</returns>
        private bool TryGetXmlAppSettingValue<T>(string index, out T? value)
        {
            value = default(T);
            if (string.IsNullOrEmpty(index) || string.IsNullOrWhiteSpace(index))
                return false;

            string[] keys = new string[] {
                            string.Format("{0}_{1}", FluentHelper.PREFIX, index),
                            string.Format("{0}:{1}", FluentHelper.PREFIX, index),
                            string.Format("{0}::{1}", FluentHelper.PREFIX, index)
                        };

            string? svalue = null;
            foreach (string key in keys)
            {
                svalue = ConfigurationManager.AppSettings[key];
                if (svalue is not null)
                    break;
            }

            if (svalue is null)
                return false;

            bool success = false;
            try
            {
                value = (T)Convert.ChangeType(svalue, typeof(T));
                success = true;
            }
            catch
            {
                success = false;
            }
            return success;
        }
#endif
    }

    /// <summary>
    /// Proporciona utilidades y acceso a configuraciones para la generación fluida de HTML en vistas fuertemente tipadas.
    /// </summary>
    /// <typeparam name="TModel">Tipo del modelo asociado al helper.</typeparam>
    public class FluentHelper<TModel> : FluentHelper
    {
#if !NETCOREAPP && !NETSTANDARD
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FluentHelper{TModel}"/> con el helper HTML especificado.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="HtmlHelper{TModel}"/> asociada al contexto de la vista.</param>
        public FluentHelper(HtmlHelper<TModel> htmlHelper)
#else
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FluentHelper{TModel}"/> con el helper HTML especificado.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper{TModel}"/> asociada al contexto de la vista.</param>
        public FluentHelper(IHtmlHelper<TModel> htmlHelper)
#endif
            : base(htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }

        /// <summary>
        /// Obtiene o establece el helper HTML fuertemente tipado asociado al contexto de la vista.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if !NETCOREAPP && !NETSTANDARD
        public new HtmlHelper<TModel> HtmlHelper
#else
        public new IHtmlHelper<TModel> HtmlHelper
#endif
        {
            get;
            set;
        }
    }
}
