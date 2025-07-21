using System;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="CheckBox"/>.
    /// Permite establecer si la casilla está marcada y si se debe incluir un campo oculto para el valor "false".
    /// </summary>
    public class CheckBoxBuilder : InputBuilderBase<CheckBox, CheckBoxBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CheckBoxBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="CheckBox"/> a asociar con el builder.</param>
        public CheckBoxBuilder(CheckBox component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece si la casilla debe aparecer marcada.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, la casilla estará marcada; si es <c>false</c>, estará desmarcada.</param>
        /// <returns>La instancia actual del builder.</returns>
        public CheckBoxBuilder Checked(bool value = true)
        {
            Component.Checked = value;

            return this;
        }

        /// <summary>
        /// Indica si se debe renderizar un campo oculto adicional para el valor "false".
        /// </summary>
        /// <param name="value">Si es <c>true</c>, se incluirá el campo oculto; si es <c>false</c>, no se incluirá.</param>
        /// <returns>La instancia actual del builder.</returns>
        public CheckBoxBuilder IncludeHidden(bool value = true)
        {
            Component.IncludeHidden = value;

            return this;
        }

    }
}