using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
#endif

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Representa un control de casilla de verificación (checkbox) con apariencia de slider (interruptor),
    /// permitiendo personalizar los textos de los estados y los atributos del slider.
    /// </summary>
    public class SliderCheckBox : CheckBox
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SliderCheckBox"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public SliderCheckBox(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            SliderAttributes = new RouteValueDictionary();
            IncludeHidden = false;
            TrueText = "On";
            FalseText = "Off";
        }

        /// <summary>
        /// Obtiene el diccionario de atributos HTML personalizados para el contenedor del slider.
        /// </summary>
        public IDictionary<string, object?> SliderAttributes { get; private set; }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará cuando el slider esté activado (valor verdadero).
        /// </summary>
        public string TrueText { get; set; }

        /// <summary>
        /// Obtiene o establece el texto que se mostrará cuando el slider esté desactivado (valor falso).
        /// </summary>
        public string FalseText { get; set; }

        /// <summary>
        /// Escribe el HTML del control slider checkbox en el escritor especificado.
        /// Incluye el checkbox, el slider visual y, si corresponde, el campo oculto.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            var checkBoxHtml = GetCheckboxHtml();

            var onBuilder = new TagBuilder("span");
            onBuilder.AddCssClass("sliderTrue");
#if !NETCOREAPP && !NETSTANDARD
            onBuilder.SetInnerText(TrueText ?? "On");
#else
            onBuilder.InnerHtml.SetContent(TrueText ?? "On");
#endif

            var offBuilder = new TagBuilder("span");
            offBuilder.AddCssClass("sliderFalse");
#if !NETCOREAPP && !NETSTANDARD
            offBuilder.SetInnerText(FalseText ?? "Off");
#else
            offBuilder.InnerHtml.SetContent(FalseText ?? "Off");
#endif

            var blockBuilder = new TagBuilder("span");
            blockBuilder.AddCssClass("sliderBlock");

#if !NETCOREAPP && !NETSTANDARD
            var sliderBuffer = new StringBuilder();
            sliderBuffer.AppendLine();
            sliderBuffer.AppendLine(onBuilder.ToString(TagRenderMode.Normal));
            sliderBuffer.AppendLine(offBuilder.ToString(TagRenderMode.Normal));
            sliderBuffer.AppendLine(blockBuilder.ToString(TagRenderMode.Normal));

            var sliderBuilder = new TagBuilder("span");
            sliderBuilder.AddCssClass("slider");
            sliderBuilder.InnerHtml = sliderBuffer.ToString();

            var labelBuffer = new StringBuilder();
            labelBuffer.AppendLine();
            labelBuffer.AppendLine(checkBoxHtml.ToString(TagRenderMode.Normal));
            labelBuffer.AppendLine(sliderBuilder.ToString(TagRenderMode.Normal));

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttributes(SliderAttributes);
            labelBuilder.AddCssClass("sliderLabel");
            labelBuilder.InnerHtml = labelBuffer.ToString();
            writer.Write(labelBuilder.ToString(TagRenderMode.Normal));
#else
            var sliderBuilder = new TagBuilder("span");
            sliderBuilder.AddCssClass("slider");
            sliderBuilder.InnerHtml.AppendHtml(onBuilder);
            sliderBuilder.InnerHtml.AppendHtml(offBuilder);
            sliderBuilder.InnerHtml.AppendHtml(blockBuilder);

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttributes(SliderAttributes);
            labelBuilder.AddCssClass("sliderLabel");
            labelBuilder.InnerHtml.AppendHtml(checkBoxHtml);
            labelBuilder.InnerHtml.AppendHtml(sliderBuilder);

            labelBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
            if (!IncludeHidden)
                return;

            // Renderiza un <input type="hidden".../> adicional para checkboxes.
            var hiddenHtml = GetHiddenHtml();
#if !NETCOREAPP && !NETSTANDARD
            writer.WriteLine(hiddenHtml.ToString(TagRenderMode.SelfClosing));
#else
            hiddenHtml.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}