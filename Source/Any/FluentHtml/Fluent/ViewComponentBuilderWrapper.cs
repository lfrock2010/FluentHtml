using System;
using AngleSharp.Dom;

#if !NET40 && !NET452 && !NET35
using AngleSharp.Html.Parser;
#else
using AngleSharp.Parser.Html;
#endif

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Wrapper para el ciclo de vida de un builder de componente de vista fluida.
    /// Permite renderizar la etiqueta de inicio y fin del componente de forma controlada y segura,
    /// asegurando la correcta disposición de recursos y el cierre de etiquetas HTML.
    /// </summary>
    /// <typeparam name="TView">Tipo del componente de vista que hereda de <see cref="ViewComponentBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto.</typeparam>
    public class ViewComponentBuilderWrapper<TView, TBuilder> : IDisposable
        where TView : ViewComponentBase
        where TBuilder : ViewComponentBuilderBase<TView, TBuilder>
    {
        private bool disposed;
        private IHtmlHelper htmlHelper;
        //private HtmlDocument htmlContent;
        private TagBuilder? tagBuilder;
        private ViewComponentBuilderBase<TView, TBuilder> viewComponentBuilder;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ViewComponentBuilderWrapper{TView, TBuilder}"/>.
        /// Renderiza la etiqueta de inicio del componente al crearse.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper"/> asociada al contexto de la vista.</param>
        /// <param name="viewComponentBuilder">Builder del componente de vista a envolver.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="htmlHelper"/> o <paramref name="viewComponentBuilder"/> es <c>null</c>.</exception>
        public ViewComponentBuilderWrapper(IHtmlHelper htmlHelper, TBuilder viewComponentBuilder)
        {
            if (htmlHelper is null)
                throw new ArgumentNullException(nameof(htmlHelper));

            if (viewComponentBuilder is null)
                throw new ArgumentNullException(nameof(viewComponentBuilder));

            this.htmlHelper = htmlHelper;
            this.viewComponentBuilder = viewComponentBuilder;
            this.Initialize();
            this.Start();
        }

        /// <summary>
        /// Finalizador que asegura la liberación de recursos y el cierre de la etiqueta HTML.
        /// </summary>
        ~ViewComponentBuilderWrapper()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Inicializa el <see cref="TagBuilder"/> a partir del HTML generado por el builder,
        /// extrayendo atributos y contenido de la primera etiqueta del documento.
        /// </summary>
        private void Initialize()
        {
            var parser = new HtmlParser();
#if !NET40 && !NET452 && !NET35
            var document = parser.ParseDocument(this.viewComponentBuilder.ToString());
#else
                var document = parser.Parse(this.viewComponentBuilder.ToString());
#endif
            IElement parentNode = document.Body.FirstElementChild;
            if (parentNode is null)
                return;

            this.tagBuilder = new TagBuilder(parentNode.NodeName);
            foreach (var attribute in parentNode.Attributes)
            {
                if (String.IsNullOrEmpty(attribute.Value) || String.IsNullOrWhiteSpace(attribute.Value))
                    continue;

                this.tagBuilder.MergeAttribute(attribute.Name, attribute.Value);
            }

#if !NETCOREAPP && !NETSTANDARD
                this.tagBuilder.InnerHtml = parentNode.InnerHtml;
#else
            this.tagBuilder.InnerHtml.AppendHtml(parentNode.InnerHtml);
#endif
        }

        /// <summary>
        /// Renderiza la etiqueta de inicio del componente en el flujo de salida de la vista.
        /// </summary>
        protected void Start()
        {
            if (this.tagBuilder is not null)
            {
#if !NETCOREAPP && !NETSTANDARD
                    this.htmlHelper.ViewContext.Writer.Write(this.tagBuilder.ToString(TagRenderMode.StartTag).Trim());
#else
                this.tagBuilder.RenderStartTag().WriteTo(this.htmlHelper.ViewContext.Writer, HtmlEncoder.Default);
#endif
            }
        }

        /// <summary>
        /// Renderiza la etiqueta de cierre del componente en el flujo de salida de la vista.
        /// </summary>
        protected void End()
        {
            if (this.tagBuilder is not null)
            {
#if !NETCOREAPP && !NETSTANDARD
                    this.htmlHelper.ViewContext.Writer.Write(this.tagBuilder.ToString(TagRenderMode.EndTag).Trim());
#else
                this.tagBuilder.RenderEndTag().WriteTo(this.htmlHelper.ViewContext.Writer, HtmlEncoder.Default);
#endif
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el wrapper y asegura el cierre de la etiqueta HTML.
        /// </summary>
        /// <param name="disposing">Indica si la liberación es explícita (<c>true</c>) o desde el finalizador (<c>false</c>).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                this.End();

            this.disposed = true;
        }

        /// <summary>
        /// Libera los recursos utilizados por el wrapper y asegura el cierre de la etiqueta HTML.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
