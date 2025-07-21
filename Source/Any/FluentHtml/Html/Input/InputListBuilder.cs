using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentHtml.Fluent;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="InputList"/>.
    /// Permite establecer clases CSS, atributos, campos de datos, valores seleccionados y enlazar colecciones de datos.
    /// </summary>
    public class InputListBuilder : SecureViewBuilderBase<InputList, InputListBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InputListBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="InputList"/> a asociar con el builder.</param>
        public InputListBuilder(InputList component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará al elemento <c>ul</c> de la lista.
        /// </summary>
        /// <param name="value">Nombre de la clase CSS.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder ListCssClass(string value)
        {
            Component.ListCssClass = value;
            return this;
        }

        /// <summary>
        /// Establece o reemplaza un atributo HTML en el elemento <c>ul</c> de la lista.
        /// </summary>
        /// <param name="name">Nombre del atributo HTML.</param>
        /// <param name="value">Valor del atributo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder ListAttribute(string name, object value)
        {
            Component.HtmlAttributes[name] = value;
            return this;
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará a cada elemento <c>li</c> de la lista.
        /// </summary>
        /// <param name="value">Nombre de la clase CSS.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder ItemCssClass(string value)
        {
            Component.ItemCssClass = value;
            return this;
        }

        /// <summary>
        /// Establece o reemplaza un atributo HTML en cada elemento <c>li</c> de la lista.
        /// </summary>
        /// <param name="name">Nombre del atributo HTML.</param>
        /// <param name="value">Valor del atributo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder ItemAttribute(string name, object value)
        {
            Component.ItemAttributes[name] = value;
            return this;
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará a cada control de entrada (<c>input</c>) de la lista.
        /// </summary>
        /// <param name="value">Nombre de la clase CSS.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder InputCssClass(string value)
        {
            Component.InputCssClass = value;
            return this;
        }

        /// <summary>
        /// Establece o reemplaza un atributo HTML en cada control de entrada (<c>input</c>) de la lista.
        /// </summary>
        /// <param name="name">Nombre del atributo HTML.</param>
        /// <param name="value">Valor del atributo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder InputAttribute(string name, object value)
        {
            Component.InputAttributes[name] = value;
            return this;
        }

        /// <summary>
        /// Establece el nombre de la propiedad que se usará como valor (<c>value</c>) de los elementos de la lista.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad del modelo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder DataValueField(string propertyName)
        {
            Component.DataValueField = propertyName;
            return this;
        }

        /// <summary>
        /// Establece el nombre de la propiedad que se usará como texto (<c>text</c>) de los elementos de la lista.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad del modelo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder DataTextField(string propertyName)
        {
            Component.DataTextField = propertyName;
            return this;
        }

        /// <summary>
        /// Asocia un campo de datos personalizado (atributo <c>data-*</c>) a una propiedad del modelo.
        /// </summary>
        /// <param name="attributeName">Nombre del atributo <c>data-*</c>.</param>
        /// <param name="propertyName">Nombre de la propiedad del modelo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder DataField(string attributeName, string propertyName)
        {
            Component.DataFields[attributeName] = propertyName;
            return this;
        }

        /// <summary>
        /// Enlaza la lista a una colección de datos.
        /// </summary>
        /// <param name="data">Colección de datos a mostrar en la lista.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder BindTo(IEnumerable data)
        {
            Component.Items = data;
            return this;
        }

        /// <summary>
        /// Enlaza la lista a una colección de datos fuertemente tipada.
        /// </summary>
        /// <typeparam name="TItem">Tipo de los elementos de la colección.</typeparam>
        /// <param name="data">Colección de datos a mostrar en la lista.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder BindTo<TItem>(IEnumerable<TItem> data)
        {
            Component.Items = data;
            return this;
        }

        /// <summary>
        /// Establece la colección de valores seleccionados en la lista.
        /// </summary>
        /// <param name="data">Colección de valores seleccionados.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder SelectedValues(IEnumerable data)
        {
            Component.SelectedValues = data;
            return this;
        }

        /// <summary>
        /// Establece la colección de valores seleccionados en la lista de forma fuertemente tipada.
        /// </summary>
        /// <typeparam name="TItem">Tipo de los elementos seleccionados.</typeparam>
        /// <param name="data">Colección de valores seleccionados.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder SelectedValues<TItem>(IEnumerable<TItem> data)
        {
            Component.SelectedValues = data;
            return this;
        }

        /// <summary>
        /// Establece un único valor seleccionado en la lista.
        /// </summary>
        /// <typeparam name="TItem">Tipo del valor seleccionado.</typeparam>
        /// <param name="data">Valor seleccionado.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder SelectedValue<TItem>(TItem data)
        {
            Component.SelectedValues = Enumerable.Repeat(data, 1);
            return this;
        }

        /// <summary>
        /// Permite configurar el mapeo de campos de la colección de datos mediante un builder especializado.
        /// </summary>
        /// <typeparam name="TItem">Tipo de los elementos de la colección.</typeparam>
        /// <param name="configurator">Acción de configuración del mapeo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public InputListBuilder Map<TItem>(Action<InputListMapBuilder<TItem>> configurator)
        {
            var builder = new InputListMapBuilder<TItem>(Component);
            configurator(builder);

            return this;
        }

    }
}