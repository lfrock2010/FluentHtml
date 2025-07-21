using FluentHtml.Fluent;

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="Element"/>.
    /// Permite establecer la etiqueta HTML, el texto, el modo de codificación y el contenido HTML del elemento.
    /// </summary>
    public class ElementBuilder : SecureViewBuilderBase<Element, ElementBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ElementBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="Element"/> a asociar con el builder.</param>
        public ElementBuilder(Element component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el nombre de la etiqueta HTML (por ejemplo, "span", "div", "label").
        /// </summary>
        /// <param name="name">Nombre de la etiqueta HTML.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ElementBuilder Tag(string name)
        {
            Component.TagName = name;
            return this;
        }

        /// <summary>
        /// Establece si el texto del elemento debe ser codificado para HTML.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, el texto será codificado; si es <c>false</c>, se permite HTML.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ElementBuilder Encode(bool value = true)
        {
            Component.Encode = value;
            return this;
        }

        /// <summary>
        /// Establece el texto que se mostrará dentro del elemento.
        /// </summary>
        /// <param name="text">Texto a mostrar en el elemento.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ElementBuilder Text(string text)
        {
            Component.Text = text;
            return this;
        }

        /// <summary>
        /// Establece el contenido HTML del elemento y deshabilita la codificación.
        /// </summary>
        /// <param name="html">HTML a mostrar en el elemento.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ElementBuilder Html(string html)
        {
            Component.Encode = false;
            Component.Text = html;
            return this;
        }
    }
}