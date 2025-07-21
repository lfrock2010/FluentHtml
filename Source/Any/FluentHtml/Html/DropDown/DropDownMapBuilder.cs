using System;
using System.Linq.Expressions;
using FluentHtml.Fluent;
using FluentHtml.Reflection;

namespace FluentHtml.Html.DropDown
{
    /// <summary>
    /// Builder fluido para mapear las propiedades de los elementos de datos a los campos de un <see cref="DropDownList"/>.
    /// Permite especificar qué propiedad del modelo se usará como valor, texto o grupo en la lista desplegable.
    /// </summary>
    /// <typeparam name="TItem">Tipo de los elementos de la colección de datos.</typeparam>
    public class DropDownMapBuilder<TItem> : BuilderBase<DropDownList, DropDownMapBuilder<TItem>>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DropDownMapBuilder{TItem}"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="DropDownList"/> a asociar con el builder.</param>
        public DropDownMapBuilder(DropDownList component)
            : base(component)
        {
        }

        /// <summary>
        /// Especifica la propiedad del modelo que se usará como valor (<c>value</c>) en los elementos de la lista.
        /// </summary>
        /// <typeparam name="TProperty">Tipo de la propiedad de valor.</typeparam>
        /// <param name="expression">Expresión lambda que identifica la propiedad de valor.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownMapBuilder<TItem> ValueField<TProperty>(Expression<Func<TItem, TProperty>> expression)
        {
            Component.DataValueField = ReflectionHelper.ExtractPropertyName(expression);
            return this;
        }

        /// <summary>
        /// Especifica la propiedad del modelo que se usará como texto (<c>text</c>) en los elementos de la lista.
        /// </summary>
        /// <typeparam name="TProperty">Tipo de la propiedad de texto.</typeparam>
        /// <param name="expression">Expresión lambda que identifica la propiedad de texto.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownMapBuilder<TItem> TextField<TProperty>(Expression<Func<TItem, TProperty>> expression)
        {
            Component.DataTextField = ReflectionHelper.ExtractPropertyName(expression);
            return this;
        }

        /// <summary>
        /// Especifica la propiedad del modelo que se usará como grupo (<c>group</c>) en los elementos de la lista.
        /// </summary>
        /// <typeparam name="TProperty">Tipo de la propiedad de grupo.</typeparam>
        /// <param name="expression">Expresión lambda que identifica la propiedad de grupo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownMapBuilder<TItem> GroupField<TProperty>(Expression<Func<TItem, TProperty>> expression)
        {
            Component.DataGroupField = ReflectionHelper.ExtractPropertyName(expression);
            return this;
        }
    }
}