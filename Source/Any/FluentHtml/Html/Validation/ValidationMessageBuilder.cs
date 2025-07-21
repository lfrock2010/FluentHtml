using System;
using FluentHtml.Fluent;

namespace FluentHtml.Html.Validation
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="ValidationMessage"/>.
    /// Permite establecer el mensaje, el campo asociado, estilos CSS y opciones de visualización.
    /// </summary>
    public class ValidationMessageBuilder : ViewComponentBuilderBase<ValidationMessage, ValidationMessageBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidationMessageBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="ValidationMessage"/> a asociar con el builder.</param>
        public ValidationMessageBuilder(ValidationMessage component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece el mensaje de validación personalizado a mostrar.
        /// </summary>
        /// <param name="value">Mensaje de validación.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ValidationMessageBuilder Message(string value)
        {
            Component.Message = value;
            return this;
        }

        /// <summary>
        /// Indica si el mensaje de validación debe mostrarse como atributo <c>title</c> en el HTML.
        /// </summary>
        /// <param name="show">Si es <c>true</c>, se mostrará el título; si es <c>false</c>, no se mostrará.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ValidationMessageBuilder ShowTitle(bool show = true)
        {
            Component.ShowTitle = show;
            return this;
        }

        /// <summary>
        /// Asocia el mensaje de validación a un campo o propiedad del modelo.
        /// </summary>
        /// <param name="value">Nombre del campo o propiedad del modelo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ValidationMessageBuilder For(string value)
        {
            Component.ModelName = value;
            return this;
        }

        /// <summary>
        /// Indica si solo se debe mostrar el icono de validación, sin el texto del mensaje.
        /// </summary>
        /// <param name="value">Si es <c>true</c>, solo se mostrará el icono; si es <c>false</c>, se mostrará el mensaje completo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ValidationMessageBuilder IconOnly(bool value = true)
        {
            Component.IconOnly = value;
            return this;
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará cuando exista un error de validación.
        /// </summary>
        /// <param name="value">Nombre de la clase CSS para el error.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ValidationMessageBuilder ErrorCssClass(string value)
        {
            Component.ValidationErrorCssClassName = value;
            return this;
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará cuando la validación sea exitosa.
        /// </summary>
        /// <param name="value">Nombre de la clase CSS para validación exitosa.</param>
        /// <returns>La instancia actual del builder.</returns>
        public ValidationMessageBuilder ValidCssClass(string value)
        {
            Component.ValidationValidCssClassName = value;
            return this;
        }
    }
}