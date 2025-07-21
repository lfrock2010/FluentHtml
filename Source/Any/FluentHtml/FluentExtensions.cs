using System;
using FluentHtml.Html;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

namespace FluentHtml
{
    /// <summary>
    /// Métodos de extensión para inicializar el helper fluido <see cref="FluentHelper"/> en vistas MVC o Razor Pages.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="FluentHelper"/> para vistas no tipadas.
        /// </summary>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper"/> asociada al contexto de la vista.</param>
        /// <returns>Instancia de <see cref="FluentHelper"/> para la vista actual.</returns>
        public static FluentHelper Fluent(this IHtmlHelper htmlHelper)
        {
            var fluent = new FluentHelper(htmlHelper);
            return fluent;
        }

#if !NETCOREAPP && !NETSTANDARD
            /// <summary>
            /// Inicializa una nueva instancia de <see cref="FluentHelper{TModel}"/> para vistas fuertemente tipadas (MVC clásico).
            /// </summary>
            /// <typeparam name="TModel">Tipo del modelo de la vista.</typeparam>
            /// <param name="htmlHelper">Instancia de <see cref="HtmlHelper{TModel}"/> asociada al contexto de la vista.</param>
            /// <returns>Instancia de <see cref="FluentHelper{TModel}"/> para la vista actual.</returns>
            public static FluentHelper<TModel> Fluent<TModel>(this HtmlHelper<TModel> htmlHelper)
#else
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="FluentHelper{TModel}"/> para vistas fuertemente tipadas (ASP.NET Core).
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo de la vista.</typeparam>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper{TModel}"/> asociada al contexto de la vista.</param>
        /// <returns>Instancia de <see cref="FluentHelper{TModel}"/> para la vista actual.</returns>
        public static FluentHelper<TModel> Fluent<TModel>(this IHtmlHelper<TModel> htmlHelper)
#endif
        {
            var fluent = new FluentHelper<TModel>(htmlHelper);
            return fluent;
        }
    }
}