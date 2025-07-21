using System;
using System.ComponentModel;

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Especifica los tipos estándar de controles de entrada HTML (&lt;input&gt;).
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// Un campo de texto de una sola línea (<c>input type="text"</c>).
        /// </summary>
        [Description("text")]
        Text,
        /// <summary>
        /// Un campo de contraseña (<c>input type="password"</c>).
        /// </summary>
        [Description("password")]
        Password,
        /// <summary>
        /// Una casilla de verificación (<c>input type="checkbox"</c>).
        /// </summary>
        [Description("checkbox")]
        Checkbox,
        /// <summary>
        /// Un botón de opción (<c>input type="radio"</c>).
        /// </summary>
        [Description("radio")]
        Radio,
        /// <summary>
        /// Un botón estándar (<c>input type="button"</c>).
        /// </summary>
        [Description("button")]
        Button,
        /// <summary>
        /// Un botón para enviar el formulario (<c>input type="submit"</c>).
        /// </summary>
        [Description("submit")]
        Submit,
        /// <summary>
        /// Un botón para restablecer el formulario (<c>input type="reset"</c>).
        /// </summary>
        [Description("reset")]
        Reset,
        /// <summary>
        /// Un campo para seleccionar archivos (<c>input type="file"</c>).
        /// </summary>
        [Description("file")]
        File,
        /// <summary>
        /// Un campo oculto (<c>input type="hidden"</c>).
        /// </summary>
        [Description("hidden")]
        Hidden,
        /// <summary>
        /// Un botón de imagen (<c>input type="image"</c>).
        /// </summary>
        [Description("image")]
        Image,

        /* HTML5 */
        /// <summary>
        /// Un campo de fecha y hora (obsoleto en HTML5) (<c>input type="datetime"</c>).
        /// </summary>
        [Description("datetime")]
        DateTime,
        /// <summary>
        /// Un campo de fecha y hora local (<c>input type="datetime-local"</c>).
        /// </summary>
        [Description("datetime-local")]
        DateTimeLocal,
        /// <summary>
        /// Un campo de fecha (<c>input type="date"</c>).
        /// </summary>
        [Description("date")]
        Date,
        /// <summary>
        /// Un campo de mes (<c>input type="month"</c>).
        /// </summary>
        [Description("month")]
        Month,
        /// <summary>
        /// Un campo de hora (<c>input type="time"</c>).
        /// </summary>
        [Description("time")]
        Time,
        /// <summary>
        /// Un campo de semana (<c>input type="week"</c>).
        /// </summary>
        [Description("week")]
        Week,
        /// <summary>
        /// Un campo numérico (<c>input type="number"</c>).
        /// </summary>
        [Description("number")]
        Number,
        /// <summary>
        /// Un control deslizante de rango (<c>input type="range"</c>).
        /// </summary>
        [Description("range")]
        Range,
        /// <summary>
        /// Un campo de correo electrónico (<c>input type="email"</c>).
        /// </summary>
        [Description("email")]
        Email,
        /// <summary>
        /// Un campo de URL (<c>input type="url"</c>).
        /// </summary>
        [Description("url")]
        Url,
        /// <summary>
        /// Un campo de búsqueda (<c>input type="search"</c>).
        /// </summary>
        [Description("search")]
        Search,
        /// <summary>
        /// Un campo de teléfono (<c>input type="tel"</c>).
        /// </summary>
        [Description("tel")]
        Tel,
        /// <summary>
        /// Un selector de color (<c>input type="color"</c>).
        /// </summary>
        [Description("color")]
        Color,
    }
}