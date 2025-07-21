using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using _HtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
using RequestContext = Microsoft.AspNetCore.Mvc.Rendering.FluentRequestContext;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para todos los componentes de vista fluida, proporcionando propiedades y métodos comunes para la integración con HTML y MVC.
    /// </summary>
    public abstract class ViewComponentBase : ComponentBase, IViewComponentBase, IHtmlString
    {
        private string? _name;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ViewComponentBase"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        protected ViewComponentBase(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            if (fluentHelper is null)
                throw new ArgumentNullException(nameof(fluentHelper));

            CssClasses = new HashSet<string>();
            ViewContext = fluentHelper.HtmlHelper.ViewContext;
#if !NETCOREAPP && !NETSTANDARD
                RequestContext = fluentHelper.HtmlHelper.ViewContext.RequestContext;
#else
            RequestContext = fluentHelper.HtmlHelper.ViewContext.GetRequestContext();
#endif
        }

        /// <summary>
        /// Obtiene el contexto de la petición asociado al componente.
        /// </summary>
        public RequestContext RequestContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Obtiene el contexto de la vista asociado al componente.
        /// </summary>
        public ViewContext ViewContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Obtiene o establece el nombre completo del campo HTML asociado al componente.
        /// </summary>
        public string? Name
        {
            get { return _name; }
            set { _name = ViewContext is not null ? ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(value) : value; }
        }

        /// <summary>
        /// Obtiene el conjunto de clases CSS asociadas al componente.
        /// </summary>
        public ISet<string> CssClasses { get; private set; }

        /// <summary>
        /// Verifica que la configuración del componente sea válida.
        /// Lanza una excepción si el nombre es nulo, vacío o contiene espacios no permitidos.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el nombre es inválido.</exception>
        public virtual void VerifySettings()
        {            
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrWhiteSpace(this.Name))
                throw new InvalidOperationException("Name cannot be blank.");

            if (!this.Name!.Contains("<#=") && this.Name.IndexOf(" ", StringComparison.Ordinal) != -1)
                throw new InvalidOperationException("Name cannot contain spaces.");
        }

        /// <summary>
        /// Establece el contexto de la vista para el componente.
        /// </summary>
        /// <param name="viewContext">El nuevo contexto de la vista.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="viewContext"/> es <c>null</c>.</exception>
        public virtual void SetViewContext(ViewContext viewContext)
        {
            if (viewContext is null)
                throw new ArgumentNullException(nameof(viewContext));

            ViewContext = viewContext;
#if !NETCOREAPP && !NETSTANDARD
                RequestContext = viewContext.RequestContext;            
#else
            RequestContext = viewContext.GetRequestContext();
#endif
        }

        /// <summary>
        /// Devuelve la representación HTML del componente como una cadena.
        /// </summary>
        /// <returns>Cadena HTML resultante del renderizado del componente.</returns>
        public virtual string ToHtmlString()
        {
            using (TextWriter writer = new StringWriter())
            {
                this.WriteTo(writer);
                return (writer.ToString() ?? string.Empty);
            }
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// Escribe el HTML del componente en el escritor especificado utilizando el codificador proporcionado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        /// <param name="encoder">Codificador HTML a utilizar.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="writer"/> o <paramref name="encoder"/> es <c>null</c>.</exception>
        public virtual void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (encoder is null)
                throw new ArgumentNullException(nameof(encoder));

            this.WriteTo(writer);
        }
#endif

        /// <summary>
        /// Devuelve la representación HTML del componente como cadena.
        /// </summary>
        /// <returns>Cadena HTML del componente.</returns>
        public override string ToString()
        {
            return ToHtmlString();
        }

        /// <summary>
        /// Escribe el HTML del componente en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public abstract void WriteTo(TextWriter writer);

        /// <summary>
        /// Fusiona los atributos HTML del componente en el <see cref="TagBuilder"/> especificado, omitiendo valores nulos por defecto.
        /// </summary>
        /// <param name="tagBuilder">Instancia de <see cref="TagBuilder"/> donde se fusionarán los atributos.</param>
        /// <param name="dropNulls">Indica si se deben omitir los atributos con valores nulos. Por defecto es <c>true</c>.</param>
        protected void MergeAttributes(TagBuilder tagBuilder, bool dropNulls = true)
        {
            var attributes = HtmlAttributes;
            MergeAttributes(tagBuilder, attributes, dropNulls);
        }

        /// <summary>
        /// Fusiona los atributos especificados en el <see cref="TagBuilder"/>, con la opción de omitir valores nulos.
        /// </summary>
        /// <param name="tagBuilder">Instancia de <see cref="TagBuilder"/> donde se fusionarán los atributos.</param>
        /// <param name="attributes">Diccionario de atributos a fusionar.</param>
        /// <param name="dropNulls">Indica si se deben omitir los atributos con valores nulos. Por defecto es <c>true</c>.</param>
        protected virtual void MergeAttributes(TagBuilder tagBuilder, IDictionary<string, object?> attributes, bool dropNulls = true)
        {
            if (!dropNulls)
            {
                tagBuilder.MergeAttributes(attributes);
                return;
            }

            // crear nuevo diccionario sin valores nulos
            var localAttributes = new Dictionary<string, object?>();

            var keys = attributes
                .Where(v => v.Value is not null)
                .Select(v => v.Key);

            foreach (string key in keys)
                localAttributes[key] = attributes[key];

            tagBuilder.MergeAttributes(localAttributes);
        }
    }
}