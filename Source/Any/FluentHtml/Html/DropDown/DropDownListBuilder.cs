using System;
using System.Collections;
using System.Collections.Generic;
using FluentHtml.Fluent;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

namespace FluentHtml.Html.DropDown
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="DropDownList"/>.
    /// Permite establecer campos de datos, valores seleccionados, atributos HTML y enlazar colecciones de datos.
    /// </summary>
    public class DropDownListBuilder : SecureViewBuilderBase<DropDownList, DropDownListBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DropDownListBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="DropDownList"/> a asociar con el builder.</param>
        public DropDownListBuilder(DropDownList component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el texto del placeholder para el control.
        /// </summary>
        /// <param name="value">Texto del placeholder.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder Placeholder(string value)
        {
            Component.Placeholder = value;
            return this;
        }

        /// <summary>
        /// Establece el nombre del campo que se usará como valor en los elementos de la lista.
        /// </summary>
        /// <param name="value">Nombre del campo de valor.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder DataValueField(string value)
        {
            Component.DataValueField = value;
            return this;
        }

        /// <summary>
        /// Establece el formato que se aplicará al valor de los elementos de la lista.
        /// </summary>
        /// <param name="value">Formato de valor.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder DataValueFormat(string value)
        {
            Component.DataValueFormat = value;
            return this;
        }

        /// <summary>
        /// Establece el nombre del campo que se usará como texto en los elementos de la lista.
        /// </summary>
        /// <param name="value">Nombre del campo de texto.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder DataTextField(string value)
        {
            Component.DataTextField = value;
            return this;
        }

        /// <summary>
        /// Establece el formato que se aplicará al texto de los elementos de la lista.
        /// </summary>
        /// <param name="value">Formato de texto.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder DataTextFormat(string value)
        {
            Component.DataTextFormat = value;
            return this;
        }

        /// <summary>
        /// Establece el nombre del campo que se usará para agrupar los elementos de la lista.
        /// </summary>
        /// <param name="value">Nombre del campo de grupo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder DataGroupField(string value)
        {
            Component.DataGroupField = value;
            return this;
        }

        /// <summary>
        /// Establece el valor seleccionado del control.
        /// </summary>
        /// <param name="value">Valor seleccionado.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder SelectedValue(object value)
        {
            Component.SelectedValue = value;
            return this;
        }

        /// <summary>
        /// Establece el valor seleccionado del control de forma fuertemente tipada.
        /// </summary>
        /// <typeparam name="TItem">Tipo del valor seleccionado.</typeparam>
        /// <param name="value">Valor seleccionado.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder SelectedValue<TItem>(TItem value)
        {
            Component.SelectedValue = value;
            return this;
        }

        /// <summary>
        /// Enlaza la lista desplegable a una colección de datos.
        /// </summary>
        /// <param name="data">Colección de datos.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder BindTo(IEnumerable data)
        {
            Component.Items = data;
            return this;
        }

        /// <summary>
        /// Enlaza la lista desplegable a una colección de <see cref="SelectListItem"/>.
        /// </summary>
        /// <param name="data">Colección de <see cref="SelectListItem"/>.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder BindTo(IEnumerable<SelectListItem> data)
        {
            Component.Items = data;
            return this;
        }

        /// <summary>
        /// Enlaza la lista desplegable a una colección de <see cref="SelectGroupItem"/>.
        /// </summary>
        /// <param name="data">Colección de <see cref="SelectGroupItem"/>.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder BindTo(IEnumerable<SelectGroupItem> data)
        {
            Component.Items = data;
            return this;
        }

        /// <summary>
        /// Enlaza la lista desplegable a una colección de datos fuertemente tipada.
        /// </summary>
        /// <typeparam name="TItem">Tipo de los elementos de la colección.</typeparam>
        /// <param name="data">Colección de datos.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder BindTo<TItem>(IEnumerable<TItem> data)
        {
            Component.Items = data;
            return this;
        }

        /// <summary>
        /// Permite configurar el mapeo de campos de la colección de datos mediante un builder especializado.
        /// </summary>
        /// <typeparam name="TItem">Tipo de los elementos de la colección.</typeparam>
        /// <param name="configurator">Acción de configuración del mapeo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder Map<TItem>(Action<DropDownMapBuilder<TItem>> configurator)
        {
            var builder = new DropDownMapBuilder<TItem>(Component);
            configurator(builder);
            return this;
        }

        /// <summary>
        /// Establece el atributo <c>title</c> del control.
        /// </summary>
        /// <param name="value">Valor del atributo <c>title</c>.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder Title(string value)
        {
            HtmlAttribute("title", "value");
            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>autofocus</c> en el control.
        /// </summary>
        /// <param name="autoFocus">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder AutoFocus(bool autoFocus = true)
        {
            const string name = "autofocus";

            if (autoFocus)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>disabled</c> en el control.
        /// </summary>
        /// <param name="disabled">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder Disabled(bool disabled = true)
        {
            const string name = "disabled";

            if (disabled)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>required</c> en el control.
        /// </summary>
        /// <param name="required">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public DropDownListBuilder Required(bool required = true)
        {
            const string name = "required";

            if (required)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

    }
}