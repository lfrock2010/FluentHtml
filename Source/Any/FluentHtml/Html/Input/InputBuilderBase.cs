using System;
using FluentHtml.Fluent;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Clase base abstracta para builders de controles de entrada HTML, proporcionando métodos para configurar tipo, valor, formato y atributos comunes.
    /// </summary>
    /// <typeparam name="TView">Tipo del componente de entrada que hereda de <see cref="InputBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto.</typeparam>
    public abstract class InputBuilderBase<TView, TBuilder>
        : SecureViewBuilderBase<TView, TBuilder>
        where TView : InputBase
        where TBuilder : InputBuilderBase<TView, TBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InputBuilderBase{TView, TBuilder}"/>.
        /// </summary>
        /// <param name="component">Componente de entrada a asociar con el builder.</param>
        protected InputBuilderBase(TView component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el tipo de entrada HTML (por ejemplo, text, password, checkbox).
        /// </summary>
        /// <param name="inputType">Tipo de entrada a utilizar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder InputType(InputType inputType)
        {
            Component.InputType = inputType;
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece el valor del control de entrada.
        /// </summary>
        /// <param name="value">Valor a asignar al control.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Value(object value)
        {
            Component.Value = value;
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece el formato que se aplicará al valor del control de entrada.
        /// </summary>
        /// <param name="format">Cadena de formato.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Format(string format)
        {
            Component.Format = format;
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece el atributo <c>title</c> del control de entrada.
        /// </summary>
        /// <param name="value">Valor del atributo <c>title</c>.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Title(string value)
        {
            HtmlAttribute("title", "value");
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>disabled</c> en el control de entrada.
        /// </summary>
        /// <param name="disabled">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Disabled(bool disabled = true)
        {
            const string name = "disabled";

            if (disabled)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>required</c> en el control de entrada.
        /// </summary>
        /// <param name="required">Si es <c>true</c>, agrega el atributo; si es <c>false</c>, lo elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Required(bool required = true)
        {
            const string name = "required";

            if (required)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return (this as TBuilder)!;
        }
    }
}