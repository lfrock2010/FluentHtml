using System;
using System.ComponentModel;

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Especifica los tipos estándar de botones HTML que se pueden utilizar en un formulario.
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// Un botón estándar que no envía el formulario al hacer clic.
        /// </summary>
        [Description("button")]
        Button,
        /// <summary>
        /// Un botón que envía el formulario al hacer clic.
        /// </summary>
        [Description("submit")]
        Submit,
        /// <summary>
        /// Un botón que restablece los valores del formulario a sus valores iniciales.
        /// </summary>
        [Description("reset")]
        Reset
    }
}