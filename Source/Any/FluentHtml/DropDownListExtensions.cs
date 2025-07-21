using System;
using System.Linq.Expressions;
using FluentHtml.Html.DropDown;

namespace FluentHtml
{
    /// <summary>
    /// Métodos de extensión para la creación fluida de controles <see cref="DropDownList"/> en vistas.
    /// </summary>
    public static class DropDownListExtensions
    {
        /// <summary>
        /// Crea un builder para un control <see cref="DropDownList"/>.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="DropDownListBuilder"/>.</returns>
        public static DropDownListBuilder DropDownList(this FluentHelper helper)
        {
            var dropDownList = new DropDownList(helper);
            var builder = new DropDownListBuilder(dropDownList);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control <see cref="DropDownList"/> enlazado a una expresión de modelo.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <returns>Instancia de <see cref="DropDownListBuilder"/>.</returns>
        public static DropDownListBuilder DropDownListFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var dropDownList = new DropDownList(helper);

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, dropDownList);
            var builder = new DropDownListBuilder(dropDownList);
            return builder;
        }
    }
}