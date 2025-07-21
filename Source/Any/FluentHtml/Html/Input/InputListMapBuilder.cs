using System;
using System.Linq.Expressions;
using FluentHtml.Fluent;
using FluentHtml.Reflection;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para mapear las propiedades de los elementos de datos a los campos de un <see cref="InputList"/>.
    /// Permite especificar qué propiedad del modelo se usará como valor, texto o atributo personalizado en la lista de entradas.
    /// </summary>
    /// <typeparam name="TItem">Tipo de los elementos de la colección de datos.</typeparam>
    public class InputListMapBuilder<TItem> : BuilderBase<InputList, InputListMapBuilder<TItem>>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InputListMapBuilder{TItem}"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="InputList"/> a asociar con el builder.</param>
        public InputListMapBuilder(InputList component)
            : base(component)
        {
        }

        /// <summary>
        /// Especifica la propiedad del modelo que se usará como valor (<c>value</c>) en los elementos de la lista.
        /// </summary>
        /// <typeparam name="TProperty">Tipo de la propiedad de valor.</typeparam>
        /// <param name="expression">Expresión lambda que identifica la propiedad de valor.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListMapBuilder<TItem> ValueField<TProperty>(Expression<Func<TItem, TProperty>> expression)
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
        public InputListMapBuilder<TItem> TextField<TProperty>(Expression<Func<TItem, TProperty>> expression)
        {
            Component.DataTextField = ReflectionHelper.ExtractPropertyName(expression);
            return this;
        }

        /// <summary>
        /// Asocia un atributo personalizado (por ejemplo, <c>data-*</c>) a una propiedad del modelo.
        /// </summary>
        /// <typeparam name="TProperty">Tipo de la propiedad del atributo.</typeparam>
        /// <param name="attributeName">Nombre del atributo personalizado.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListMapBuilder<TItem> AttributeField<TProperty>(string attributeName, Expression<Func<TItem, TProperty>> expression)
        {
            Component.DataFields[attributeName] = ReflectionHelper.ExtractPropertyName(expression);
            return this;
        }
    }
}