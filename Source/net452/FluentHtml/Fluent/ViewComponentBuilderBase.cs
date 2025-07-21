using System;
using System.Linq.Expressions;
using System.Web;
using FluentHtml.Reflection;
using System.Web.Mvc;
using System.Web.UI;

namespace FluentHtml.Fluent
{
    public abstract class ViewComponentBuilderBase<TView, TBuilder> : BuilderBase<TView, TBuilder>, IHtmlString
        where TView : ViewComponentBase
        where TBuilder : ViewComponentBuilderBase<TView, TBuilder>
    {
        protected ViewComponentBuilderBase(TView component)
            : base(component)
        {
        }

        public virtual TBuilder Name(string componentName)
        {
            Component.Name = componentName;
            this.Id(componentName);
            return this as TBuilder;
        }

        public TBuilder Name<TItem, TProperty>(Expression<Func<TItem, TProperty>> expression)
        {
            Component.Name = ReflectionHelper.ExtractPropertyName(expression);
            return this as TBuilder;
        }

        public TBuilder CssClass(params string[] values)
        {
            foreach (String cssClass in values)
                Component.CssClasses.Add(cssClass);

            return this as TBuilder;
        }        

        public virtual TView ToComponent()
        {
            return Component;
        }

        public static implicit operator TView(ViewComponentBuilderBase<TView, TBuilder> builder)
        {
            return builder.ToComponent();
        }

        public ViewComponentBuilderWrapper<TView, TBuilder> Begin()
        {            
            return new ViewComponentBuilderWrapper<TView, TBuilder>(this.Component.HtmlHelper, this as TBuilder);
        }

        public string ToHtmlString()
        {
            return ToComponent().ToHtmlString();
        }

        public override string ToString()
        {
            return this.ToHtmlString().ToString();
        }

        public void Render()            
        {            
            HtmlHelper htmlHelper = this.Component.HtmlHelper;

            if (htmlHelper != null)
            {
                ViewContext viewContext = htmlHelper.ViewContext;
                if (viewContext != null)
                {
                    HtmlTextWriter writer = HttpContext.Current.Request.Browser.CreateHtmlTextWriter(viewContext.Writer);
                    String content = this.ToComponent().ToHtmlString();
                    writer.WriteLine(content);
                }
            }
        }

        public MvcHtmlString GetHtml()           
        {
            this.Render();
            return MvcHtmlString.Create(String.Empty);            
        }
    }
}
