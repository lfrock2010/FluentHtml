using System;
using FluentHtml.Fluent;

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="Button"/>.
    /// Permite establecer el texto, el tipo, el modo de codificación y atributos HTML del botón.
    /// </summary>
    public class ButtonBuilder : SecureViewBuilderBase<Button, ButtonBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ButtonBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="Button"/> a asociar con el builder.</param>
        public ButtonBuilder(Button component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece si el texto del botón debe ser codificado para HTML.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, el texto será codificado; si es <c>false</c>, se permite HTML.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ButtonBuilder Encode(bool value = true)
        {
            Component.Encode = value;
            return this;
        }

        /// <summary>
        /// Establece el texto del botón y habilita la codificación HTML.
        /// </summary>
        /// <param name="text">Texto a mostrar en el botón.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ButtonBuilder Text(string text)
        {
            Component.Encode = true;
            Component.Text = text;
            return this;
        }

        /// <summary>
        /// Establece el contenido HTML del botón y deshabilita la codificación.
        /// </summary>
        /// <param name="html">HTML a mostrar en el botón.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ButtonBuilder Html(string html)
        {
            Component.Encode = false;
            Component.Text = html;
            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>disabled</c> en el botón.
        /// </summary>
        /// <param name="disabled">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ButtonBuilder Disabled(bool disabled = true)
        {
            const string name = "disabled";

            if (disabled)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece el tipo de botón (button, submit, reset).
        /// </summary>
        /// <param name="buttonType">Tipo de botón a utilizar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ButtonBuilder Type(ButtonType buttonType)
        {
            Component.ButtonType = buttonType;
            return this;
        }
    }
}