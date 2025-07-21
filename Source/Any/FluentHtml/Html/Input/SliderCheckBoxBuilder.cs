using System;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="SliderCheckBox"/>.
    /// Permite establecer el estado, los textos de los valores y atributos personalizados del slider.
    /// </summary>
    public class SliderCheckBoxBuilder : InputBuilderBase<SliderCheckBox, SliderCheckBoxBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SliderCheckBoxBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="SliderCheckBox"/> a asociar con el builder.</param>
        public SliderCheckBoxBuilder(SliderCheckBox component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece si el slider debe aparecer marcado (<c>checked</c>).
        /// </summary>
        /// <param name="value">Si es <c>true</c>, el slider estará activado; si es <c>false</c>, desactivado.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SliderCheckBoxBuilder Checked(bool value = true)
        {
            Component.Checked = value;
            return this;
        }

        /// <summary>
        /// Indica si se debe incluir un campo oculto para el valor "false" al renderizar el slider.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, se incluirá el campo oculto; si es <c>false</c>, no se incluirá.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SliderCheckBoxBuilder IncludeHidden(bool value = true)
        {
            Component.IncludeHidden = value;
            return this;
        }

        /// <summary>
        /// Establece el texto que se mostrará cuando el slider esté activado (valor verdadero).
        /// </summary>
        /// <param name="value">Texto para el estado "true".</param>
        /// <returns>La instancia actual del builder.</returns>
        public SliderCheckBoxBuilder TrueText(string value)
        {
            Component.TrueText = value;
            return this;
        }

        /// <summary>
        /// Establece el texto que se mostrará cuando el slider esté desactivado (valor falso).
        /// </summary>
        /// <param name="value">Texto para el estado "false".</param>
        /// <returns>La instancia actual del builder.</returns>
        public SliderCheckBoxBuilder FalseText(string value)
        {
            Component.FalseText = value;
            return this;
        }

        /// <summary>
        /// Establece un atributo HTML personalizado para el contenedor del slider.
        /// </summary>
        /// <param name="name">Nombre del atributo.</param>
        /// <param name="value">Valor del atributo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual SliderCheckBoxBuilder SliderAttribute(string name, object value)
        {
            Component.SliderAttributes[name] = value;
            return this;
        }
    }
}