using System;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="TextArea"/>.
    /// Permite establecer atributos HTML estándar como columnas, filas, placeholder, envoltura, solo lectura, autoenfoque, autocompletado y longitud máxima.
    /// </summary>
    public class TextAreaBuilder : InputBuilderBase<TextArea, TextAreaBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TextAreaBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="TextArea"/> a asociar con el builder.</param>
        public TextAreaBuilder(TextArea component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el número de columnas (<c>cols</c>) del área de texto.
        /// </summary>
        /// <param name="value">Cantidad de columnas.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder Columns(int value)
        {
            HtmlAttribute("cols", value);
            return this;
        }

        /// <summary>
        /// Establece el número de filas (<c>rows</c>) del área de texto.
        /// </summary>
        /// <param name="value">Cantidad de filas.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder Rows(int value)
        {
            HtmlAttribute("rows", value);
            return this;
        }

        /// <summary>
        /// Establece el texto de placeholder (<c>placeholder</c>) del área de texto.
        /// </summary>
        /// <param name="value">Texto de placeholder.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder Placeholder(string value)
        {
            HtmlAttribute("placeholder", value ?? string.Empty);
            return this;
        }

        /// <summary>
        /// Establece el modo de envoltura (<c>wrap</c>) del área de texto.
        /// </summary>
        /// <param name="value">Valor del atributo wrap (por ejemplo, "soft" o "hard").</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder Wrap(string value)
        {
            HtmlAttribute("wrap", value ?? string.Empty);
            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>readonly</c> en el área de texto.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder Readonly(bool value = true)
        {
            const string name = "readonly";

            if (value)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>autofocus</c> en el área de texto.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder AutoFocus(bool value = true)
        {
            const string name = "autofocus";

            if (value)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece el atributo <c>autocomplete</c> en el área de texto.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, establece "on"; si es <c>false</c>, establece "off".</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder AutoComplete(bool value = true)
        {
            HtmlAttribute("autocomplete", value ? "on" : "off");
            return this;
        }

        /// <summary>
        /// Establece el atributo <c>maxlength</c> en el área de texto.
        /// </summary>
        /// <param name="value">Longitud máxima permitida.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TextAreaBuilder MaxLength(int value)
        {
            HtmlAttribute("maxlength ", value);
            return this;
        }
    }
}