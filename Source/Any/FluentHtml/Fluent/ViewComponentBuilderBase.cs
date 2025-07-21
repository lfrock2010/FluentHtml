using System;
using System.Linq.Expressions;
using FluentHtml.Reflection;
using System.IO;

#if !NETCOREAPP && !NETSTANDARD
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif


namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para los builders de componentes de vista, proporcionando métodos para configurar el nombre, clases CSS y renderizado del componente.
    /// </summary>
    /// <typeparam name="TView">Tipo del componente de vista que hereda de <see cref="ViewComponentBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto.</typeparam>
    public abstract class ViewComponentBuilderBase<TView, TBuilder> : BuilderBase<TView, TBuilder>, IHtmlString
        where TView : ViewComponentBase
        where TBuilder : ViewComponentBuilderBase<TView, TBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ViewComponentBuilderBase{TView, TBuilder}"/>.
        /// </summary>
        /// <param name="component">Componente de vista a asociar con el builder.</param>
        protected ViewComponentBuilderBase(TView component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el nombre del componente y su atributo <c>id</c>.
        /// </summary>
        /// <param name="componentName">Nombre a asignar al componente.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder Name(string componentName)
        {
            Component.Name = componentName;
            this.Id(componentName);
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece el nombre del componente a partir de una expresión lambda fuertemente tipada.
        /// </summary>
        /// <typeparam name="TItem">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="expression">Expresión lambda que identifica la propiedad.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Name<TItem, TProperty>(Expression<Func<TItem, TProperty>> expression)
        {
            Component.Name = ReflectionHelper.ExtractPropertyName(expression);
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Agrega una o más clases CSS al componente.
        /// </summary>
        /// <param name="values">Lista de clases CSS a agregar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder CssClass(params string[] values)
        {
            foreach (String cssClass in values)
                Component.CssClasses.Add(cssClass);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Devuelve la instancia del componente asociado a este builder.
        /// </summary>
        /// <returns>El componente de vista.</returns>
        public virtual TView ToComponent()
        {
            return Component;
        }

        /// <summary>
        /// Conversión implícita de un builder a su componente asociado.
        /// </summary>
        /// <param name="builder">Instancia del builder.</param>
        public static implicit operator TView(ViewComponentBuilderBase<TView, TBuilder> builder)
        {
            return builder.ToComponent();
        }

        /// <summary>
        /// Inicia el renderizado del componente y devuelve un wrapper para el ciclo de vida del builder.
        /// </summary>
        /// <returns>Instancia de <see cref="ViewComponentBuilderWrapper{TView, TBuilder}"/>.</returns>
        public ViewComponentBuilderWrapper<TView, TBuilder> Begin()
        {
            return new ViewComponentBuilderWrapper<TView, TBuilder>(this.Component.HtmlHelper, (this as TBuilder)!);
        }

        /// <summary>
        /// Renderiza el componente a una cadena HTML.
        /// </summary>
        /// <returns>Cadena HTML resultante del renderizado del componente.</returns>
        public string ToHtmlString()
        {
            using (TextWriter writer = new StringWriter())
            {
                this.WriteTo(writer);
                return writer.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Escribe el HTML del componente en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public virtual void WriteTo(TextWriter writer)
        {
            this.ToComponent().WriteTo(writer);
        }

        /// <summary>
        /// Devuelve la representación HTML del componente como cadena.
        /// </summary>
        /// <returns>Cadena HTML del componente.</returns>
        public override string ToString()
        {
            return this.ToHtmlString();
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// Escribe el HTML del componente en el escritor especificado utilizando el codificador proporcionado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        /// <param name="encoder">Codificador HTML a utilizar.</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="writer"/> o <paramref name="encoder"/> es <c>null</c>.</exception>
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
        /// Renderiza el componente en el contexto de vista actual.
        /// </summary>
        public void Render()
        {
            IHtmlHelper htmlHelper = this.Component.HtmlHelper;

            if (htmlHelper is not null)
            {
                ViewContext viewContext = htmlHelper.ViewContext;
                if (viewContext is not null)
                {
#if !NETCOREAPP && !NETSTANDARD
                        HtmlTextWriter writer = HttpContext.Current.Request.Browser.CreateHtmlTextWriter(viewContext.Writer);
                        this.ToComponent().WriteTo(writer);
#else
                    var writer = htmlHelper.ViewContext.Writer;
                    var encoder = HtmlEncoder.Default;
                    this.ToComponent().WriteTo(writer);
#endif
                }
            }
        }

#if !NETCOREAPP && !NETSTANDARD
            /// <summary>
            /// Renderiza el componente y devuelve una instancia vacía de <see cref="MvcHtmlString"/>.
            /// </summary>
            /// <returns>Instancia de <see cref="MvcHtmlString"/>.</returns>
            public MvcHtmlString GetHtml()           
            {
                this.Render();
                return MvcHtmlString.Create(String.Empty);            
            }
#else
        /// <summary>
        /// Renderiza el componente y devuelve una instancia vacía de <see cref="IHtmlString"/>.
        /// </summary>
        /// <returns>Instancia de <see cref="IHtmlString"/>.</returns>
        public IHtmlString GetHtml()
        {
            this.Render();
            StringHtmlContent content = new StringHtmlContent(string.Empty);
            return content;
        }
#endif
    }
}
