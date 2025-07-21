using System;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="RadioButton"/>.
    /// Permite establecer el estado seleccionado del botón de opción.
    /// </summary>
    public class RadioButtonBuilder : InputBuilderBase<RadioButton, RadioButtonBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RadioButtonBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="RadioButton"/> a asociar con el builder.</param>
        public RadioButtonBuilder(RadioButton component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece si el botón de opción debe aparecer seleccionado (<c>checked</c>).
        /// </summary>
        /// <param name="value">Si es <c>true</c>, el botón estará seleccionado; si es <c>false</c>, no lo estará.</param>
        /// <returns>La instancia actual del builder.</returns>
        public RadioButtonBuilder Checked(bool value = true)
        {
            Component.Checked = value;
            return this;
        }
    }
}