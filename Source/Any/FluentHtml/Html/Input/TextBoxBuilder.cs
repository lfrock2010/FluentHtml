using System;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="TextBox"/>.
    /// Permite establecer atributos HTML comunes como placeholder, readonly, autofocus, autocomplete y maxlength.
    /// </summary>
    public class TextBoxBuilder : InputBuilderBase<TextBox, TextBoxBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TextBoxBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="TextBox"/> a asociar con el builder.</param>
        public TextBoxBuilder(TextBox component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el atributo <c>placeholder</c> del input.
        /// </summary>
        /// <param name="value">Texto a mostrar como placeholder.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextBoxBuilder Placeholder(string value)
        {
            HtmlAttribute("placeholder", value ?? string.Empty);
            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>readonly</c> en el input.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextBoxBuilder Readonly(bool value = true)
        {
            const string name = "readonly";

            if (value)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>autofocus</c> en el input.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextBoxBuilder AutoFocus(bool value = true)
        {
            const string name = "autofocus";

            if (value)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece el atributo <c>autocomplete</c> en el input.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, el valor será "on"; si es <c>false</c>, será "off".</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextBoxBuilder AutoComplete(bool value = true)
        {
            HtmlAttribute("autocomplete", value ? "on" : "off");
            return this;
        }

        /// <summary>
        /// Establece el atributo <c>maxlength</c> en el input.
        /// </summary>
        /// <param name="value">Número máximo de caracteres permitidos.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextBoxBuilder MaxLength(int value)
        {
            HtmlAttribute("maxlength ", value);
            return this;
        }
    }
}