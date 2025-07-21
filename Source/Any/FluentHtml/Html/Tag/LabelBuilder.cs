using System;
using FluentHtml.Extensions;
using FluentHtml.Fluent;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="Label"/>.
    /// Permite establecer el texto, el HTML, el atributo <c>for</c>, la codificación y el nombre del componente.
    /// </summary>
    public class LabelBuilder : SecureViewBuilderBase<Label, LabelBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="LabelBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="Label"/> a asociar con el builder.</param>
        public LabelBuilder(Label component)
           : base(component)
        {
        }

        /// <summary>
        /// Establece si el texto de la etiqueta debe ser codificado como HTML.
        /// </summary>
        /// <param name="value">Valor que indica si se debe codificar el texto (por defecto <c>true</c>).</param>
        /// <returns>La instancia actual del builder.</returns>
        public LabelBuilder Encode(bool value = true)
        {
            Component.Encode = value;
            return this;
        }

        /// <summary>
        /// Establece el texto que se mostrará dentro de la etiqueta <c>&lt;label&gt;</c>.
        /// </summary>
        /// <param name="text">Texto a mostrar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LabelBuilder Text(string text)
        {
            Component.Text = text;
            return this;
        }

        /// <summary>
        /// Establece el contenido HTML que se mostrará dentro de la etiqueta <c>&lt;label&gt;</c> (sin codificar).
        /// </summary>
        /// <param name="html">HTML a mostrar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LabelBuilder Html(string html)
        {
            Component.Encode = false;
            Component.Text = html;
            return this;
        }

        /// <summary>
        /// Establece el valor del atributo <c>for</c> de la etiqueta <c>&lt;label&gt;</c>,
        /// asociando la etiqueta a un control de formulario.
        /// </summary>
        /// <param name="name">Nombre del control al que se asociará la etiqueta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LabelBuilder For(string name)
        {
            Component.For = name;
            return this;
        }

        /// <summary>
        /// Establece el nombre del componente y su atributo <c>id</c>.
        /// </summary>
        /// <param name="componentName">Nombre a asignar al componente.</param>
        /// <returns>La instancia actual del builder.</returns>
        public override LabelBuilder Name(string componentName)
        {
            Component.Name = componentName;
            return this;
        }
    }
}
