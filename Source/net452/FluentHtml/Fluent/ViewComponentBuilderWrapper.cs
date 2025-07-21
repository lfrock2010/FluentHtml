using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FluentHtml.Fluent
{
    public class ViewComponentBuilderWrapper<TView, TBuilder> : IDisposable
        where TView : ViewComponentBase
        where TBuilder : ViewComponentBuilderBase<TView, TBuilder>
    {
        private bool disposed;
        private HtmlHelper htmlHelper;
        //private HtmlDocument htmlContent;
        private TagBuilder tagBuilder;
        private ViewComponentBuilderBase<TView, TBuilder> viewComponentBuilder;

        public ViewComponentBuilderWrapper(HtmlHelper htmlHelper, TBuilder viewComponentBuilder)
        {
            if (Object.ReferenceEquals(htmlHelper, null))
                throw new ArgumentNullException("htmlHelper");

            if (Object.ReferenceEquals(viewComponentBuilder, null))
                throw new ArgumentNullException("viewComponentBuilder");

            this.htmlHelper = htmlHelper;
            this.viewComponentBuilder = viewComponentBuilder;
            this.Initialize();
            this.Start();
        }

        ~ViewComponentBuilderWrapper()
        {
            this.Dispose(false);
        }

        private void Initialize()
        {
            HtmlDocument htmlContent = new HtmlDocument();
            htmlContent.LoadHtml(this.viewComponentBuilder.ToString());

            HtmlNode parentNode = htmlContent.DocumentNode.FirstChild;
            if (parentNode == null)
                return;            
            
            this.tagBuilder = new TagBuilder(parentNode.Name);
            foreach (HtmlAttribute attribute in parentNode.Attributes)
            {
                if (String.IsNullOrEmpty(attribute.Value) || String.IsNullOrWhiteSpace(attribute.Value))
                    continue;               

                this.tagBuilder.MergeAttribute(attribute.Name, attribute.Value);
            }

            this.tagBuilder.InnerHtml = parentNode.InnerHtml;
        }

        protected void Start()
        {
            if (this.tagBuilder != null)
                this.htmlHelper.ViewContext.Writer.Write(this.tagBuilder.ToString(TagRenderMode.StartTag).Trim());            
        }

        protected void End()
        {
            if (this.tagBuilder != null)
                this.htmlHelper.ViewContext.Writer.Write(this.tagBuilder.ToString(TagRenderMode.EndTag).Trim());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)            
                this.End();            

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
