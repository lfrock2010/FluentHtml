using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using FluentHtml.Extensions;
using FluentHtml.Fluent;
using FluentHtml.Reflection;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System.Text.Encodings.Web;
#endif

namespace FluentHtml.Html.Input
{
    /// <summary>
    /// Representa una lista de controles de entrada HTML (checkbox, radio, etc.) configurable y segura,
    /// permitiendo personalizar atributos, clases CSS, campos de datos y valores seleccionados.
    /// </summary>
    public class InputList : SecureViewBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InputList"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public InputList(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            ItemAttributes = new RouteValueDictionary();
            LabelAttributes = new RouteValueDictionary();
            InputAttributes = new RouteValueDictionary();
            DataFields = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Obtiene o establece el tipo de entrada HTML para los elementos de la lista (checkbox, radio, etc.).
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// Obtiene o establece la clase CSS que se aplicará al elemento <c>ul</c> de la lista.
        /// </summary>
        public string? ListCssClass { get; set; }

        /// <summary>
        /// Obtiene el diccionario de atributos HTML personalizados para cada elemento <c>li</c> de la lista.
        /// </summary>
        public IDictionary<string, object?> ItemAttributes { get; private set; }

        /// <summary>
        /// Obtiene o establece la clase CSS que se aplicará a cada elemento <c>li</c> de la lista.
        /// </summary>
        public string? ItemCssClass { get; set; }

        /// <summary>
        /// Obtiene el diccionario de atributos HTML personalizados para cada etiqueta <c>label</c> de la lista.
        /// </summary>
        public IDictionary<string, object?> LabelAttributes { get; private set; }

        /// <summary>
        /// Obtiene o establece la clase CSS que se aplicará a cada etiqueta <c>label</c> de la lista.
        /// </summary>
        public string? LabelCssClass { get; set; }

        /// <summary>
        /// Obtiene el diccionario de atributos HTML personalizados para cada control de entrada (<c>input</c>) de la lista.
        /// </summary>
        public IDictionary<string, object?> InputAttributes { get; private set; }

        /// <summary>
        /// Obtiene o establece la clase CSS que se aplicará a cada control de entrada (<c>input</c>) de la lista.
        /// </summary>
        public string? InputCssClass { get; set; }

        /// <summary>
        /// Obtiene el diccionario de campos de datos personalizados (atributos <c>data-*</c>) para los elementos de la lista.
        /// </summary>
        public IDictionary<string, string?> DataFields { get; private set; }

        /// <summary>
        /// Obtiene o establece el nombre de la propiedad que se usará como valor (<c>value</c>) de los elementos de la lista.
        /// </summary>
        public string? DataValueField { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la propiedad que se usará como texto (<c>text</c>) de los elementos de la lista.
        /// </summary>
        public string? DataTextField { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de elementos que se mostrarán en la lista.
        /// </summary>
        public IEnumerable? Items { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de valores seleccionados en la lista.
        /// </summary>
        public IEnumerable? SelectedValues { get; set; }

        /// <summary>
        /// Escribe el HTML de la lista de controles de entrada en el escritor especificado.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            var orderList = new TagBuilder("ul");
            if (!this.HtmlAttributes.ContainsKey("id") && !string.IsNullOrEmpty(this.Name))
#if !NETCOREAPP && !NETSTANDARD
                    orderList.GenerateId(Name);
#else
                orderList.GenerateId(Name, this.HtmlHelper.IdAttributeDotReplacement);
#endif

            MergeAttributes(orderList, HtmlAttributes);

            string listClass = ListCssClass ?? string.Format("{0}-list", InputType);
            orderList.AddCssClass(listClass);

            var selectedValues = new HashSet<object>();
            if (SelectedValues is not null)
                selectedValues.UnionWith(SelectedValues.Cast<object>());

            // make sure not null
            var items = Items ?? Enumerable.Empty<object>();

            foreach (var item in items)
            {
                if (item is null)
                    continue;

                var inputItem = EnumItem<InputType>.Create(InputType);
                string? inputType = inputItem?.Description?.ToLowerInvariant();

                var text = DataTextField.HasValue() ? LateBinder.GetProperty(item, DataTextField ?? string.Empty) : item;
                var textString = Convert.ToString(text) ?? string.Empty;

                var value = DataValueField.HasValue() ? LateBinder.GetProperty(item, DataValueField ?? string.Empty) : text;
                var valueString = Convert.ToString(value);

                bool selected = selectedValues.Contains(value) || selectedValues.Contains(valueString ?? string.Empty);

                // resolve data fields
                var dataAttributes = new Dictionary<string, object?>();
                foreach (var dataField in DataFields)
                {
                    if (string.IsNullOrEmpty(dataField.Value) || dataField.Value is null)
                        continue;

                    var dataValue = LateBinder.GetProperty(item, dataField.Value);
                    if (dataValue is not null)
                        dataAttributes[dataField.Key] = dataValue;
                }

                var input = new TagBuilder("input");
                input.MergeAttribute("type", inputType);
                input.MergeAttribute("name", Name);
                input.MergeAttribute("value", valueString);
                if (selected)
                    input.MergeAttribute("checked", "checked");

                if (InputAttributes is not null)
                    MergeAttributes(input, InputAttributes);

                if (InputCssClass.HasValue())
                    input.AddCssClass(InputCssClass!);

                // merge data attributes
                MergeAttributes(input, dataAttributes);

                if (!CanWrite())
                {
                    input.MergeAttribute("readonly", "readonly", true);
                    if (DeniedClass.HasValue())
                        input.AddCssClass(DeniedClass);
                }
                else if (GrantedClass.HasValue())
                {
                    input.AddCssClass(GrantedClass!);
                }

                var label = new TagBuilder("label");
                if (LabelAttributes is not null)
                    MergeAttributes(label, LabelAttributes);

                if (LabelCssClass.HasValue())
                    label.AddCssClass(LabelCssClass!);

                // merge data attributes
                MergeAttributes(label, dataAttributes);

#if !NETCOREAPP && !NETSTANDARD
                    label.InnerHtml = "{0} {1}".FormatWith(
                        input.ToString(TagRenderMode.SelfClosing),
                        HttpUtility.HtmlEncode(textString));
#else
                label.InnerHtml.AppendHtml(input);
                label.InnerHtml.Append(" ");
                label.InnerHtml.Append(textString);
#endif

                var listItem = new TagBuilder("li");

                if (ItemAttributes is not null)
                    MergeAttributes(listItem, ItemAttributes);

#if !NETCOREAPP && !NETSTANDARD
                    listItem.InnerHtml = label.ToString(TagRenderMode.Normal);                
                    orderList.InnerHtml = listItem.ToString(TagRenderMode.Normal);
#else
                listItem.InnerHtml.AppendHtml(label);
                orderList.InnerHtml.AppendHtml(listItem);
#endif
            }

#if !NETCOREAPP && !NETSTANDARD
                writer.Write(orderList.ToString(TagRenderMode.Normal));
#else
            orderList.WriteTo(writer, HtmlEncoder.Default);
#endif
        }
    }
}
