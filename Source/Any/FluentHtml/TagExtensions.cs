using System;
using System.Linq.Expressions;
using FluentHtml.Html.Tag;

namespace FluentHtml
{
    /// <summary>
    /// Métodos de extensión para <see cref="FluentHelper"/> que facilitan la creación fluida de etiquetas HTML comunes
    /// como label, link, button y span, devolviendo sus builders correspondientes.
    /// </summary>
    public static class TagExtensions
    {
        /// <summary>
        /// Crea un builder para una etiqueta <c>&lt;label&gt;</c> sin asociarla a un modelo.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="LabelBuilder"/> para configuración fluida.</returns>
        public static LabelBuilder Label(this FluentHelper helper)
        {
            var component = new Label(helper);
            var builder = new LabelBuilder(component);

            return builder;
        }

        /// <summary>
        /// Crea un builder para una etiqueta <c>&lt;label&gt;</c> asociada a una propiedad de un modelo fuertemente tipado.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <returns>Instancia de <see cref="LabelBuilder"/> para configuración fluida.</returns>
        public static LabelBuilder LabelFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new Label(helper);

            // Asigna la metadata de la propiedad al componente label
            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new LabelBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un enlace HTML <c>&lt;a&gt;</c>.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="LinkBuilder"/> para configuración fluida.</returns>
        public static LinkBuilder Link(this FluentHelper helper)
        {
            var component = new Link(helper);
            var builder = new LinkBuilder(component);

            return builder;
        }

        /// <summary>
        /// Crea un builder para un botón HTML <c>&lt;button&gt;</c>.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="ButtonBuilder"/> para configuración fluida.</returns>
        public static ButtonBuilder Button(this FluentHelper helper)
        {
            var component = new Button(helper);
            var builder = new ButtonBuilder(component);

            return builder;
        }

        /// <summary>
        /// Crea un builder para un elemento HTML <c>&lt;span&gt;</c>.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="ElementBuilder"/> para configuración fluida.</returns>
        public static ElementBuilder Span(this FluentHelper helper)
        {
            var component = new Element(helper) { TagName = "span" };

            var builder = new ElementBuilder(component);
            return builder;
        }
    }
}
