using FluentHtml.Extensions;
using FluentHtml.Fluent;
using FluentHtml.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
#else
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#endif

namespace FluentHtml.Html.DropDown
{
    /// <summary>
    /// Representa un control de lista desplegable (<c>&lt;select&gt;</c>) configurable y seguro,
    /// permitiendo personalizar los campos de datos, el valor seleccionado, los elementos y la agrupación.
    /// </summary>
    public class DropDownList : SecureViewBase, IHasModelData
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if !NETCOREAPP && !NETSTANDARD
        private ModelMetadata? _metadata;
#else
        private ModelExplorer? _metadata;
#endif

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DropDownList"/>.
        /// </summary>
        /// <param name="fluentHelper">Instancia de <see cref="FluentHelper"/> asociada al componente.</param>
        public DropDownList(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
        }

        /// <summary>
        /// Obtiene o establece la metadata del modelo asociada al componente (MVC clásico o ASP.NET Core).
        /// Al establecerse, también actualiza el valor seleccionado si la metadata no es nula.
        /// </summary>
#if !NETCOREAPP && !NETSTANDARD
        public ModelMetadata? Metadata 
#else
        public ModelExplorer? Metadata
#endif
        {
            get
            {
                return this._metadata;
            }
            set
            {
                if (this._metadata != value)
                {
                    this._metadata = value;
                    if (this._metadata is not null)
                        this.SelectedValue = this._metadata.Model;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el texto del elemento placeholder (opción vacía) que se mostrará al inicio de la lista.
        /// </summary>
        public string? Placeholder { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la propiedad que se usará como valor (<c>value</c>) de los elementos de la lista.
        /// </summary>
        public string? DataValueField { get; set; }
        /// <summary>
        /// Obtiene o establece el formato de visualización para el valor de los elementos de la lista.
        /// </summary>
        public string? DataValueFormat { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la propiedad que se usará como texto (<c>text</c>) de los elementos de la lista.
        /// </summary>
        public string? DataTextField { get; set; }
        /// <summary>
        /// Obtiene o establece el formato de visualización para el texto de los elementos de la lista.
        /// </summary>
        public string? DataTextFormat { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la propiedad que se usará para agrupar los elementos de la lista.
        /// </summary>
        public string? DataGroupField { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de elementos que se mostrarán en la lista.
        /// </summary>
        public IEnumerable? Items { get; set; }

        /// <summary>
        /// Obtiene o establece el valor seleccionado en la lista.
        /// </summary>
        public object? SelectedValue { get; set; }

        /// <summary>
        /// Escribe el HTML de la lista desplegable en el escritor especificado.
        /// Incluye los elementos <c>&lt;option&gt;</c>, atributos, clases CSS, validación y control de permisos.
        /// </summary>
        /// <param name="writer">Instancia de <see cref="TextWriter"/> donde se escribirá el HTML.</param>
        public override void WriteTo(TextWriter writer)
        {
            VerifySettings();

            string? fullName = Name;
            var options = BuildOptionHtml();

            var selectBuilder = new TagBuilder("select");
#if !NETCOREAPP && !NETSTANDARD
            StringBuilder sb = new StringBuilder();
            foreach (var option in options)
                sb.AppendLine(option.ToString(TagRenderMode.Normal));

            selectBuilder.InnerHtml = sb.ToString();
#else
            foreach (var option in options)
                selectBuilder.InnerHtml.AppendHtml(option);
#endif

            MergeAttributes(selectBuilder);
            selectBuilder.MergeAttribute("name", fullName, true);

            if (!this.HtmlAttributes.ContainsKey("id") && !string.IsNullOrEmpty(fullName))
#if !NETCOREAPP && !NETSTANDARD
                selectBuilder.GenerateId(fullName);
#else
                selectBuilder.GenerateId(fullName, this.HtmlHelper.IdAttributeDotReplacement);
#endif

            foreach (string cssClass in CssClasses)
                selectBuilder.AddCssClass(cssClass);

            if (HtmlHelper is null)
            {
#if !NETCOREAPP && !NETSTANDARD
                writer.Write(selectBuilder.ToString(TagRenderMode.Normal));
#else
                selectBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
                return;
            }

            // If there are any errors for a named field, we add the css attribute.
            this.MergeValidationAttributes(selectBuilder, this.Metadata);

            if (!CanWrite())
            {
                selectBuilder.MergeAttribute("disabled", "disabled", true);
                if (DeniedClass.HasValue())
                    selectBuilder.AddCssClass(DeniedClass);
            }
            else if (GrantedClass.HasValue())
                selectBuilder.AddCssClass(GrantedClass ?? string.Empty);

#if !NETCOREAPP && !NETSTANDARD
            writer.Write(selectBuilder.ToString(TagRenderMode.Normal));            
#else
            selectBuilder.WriteTo(writer, HtmlEncoder.Default);
#endif
        }

        /// <summary>
        /// Construye la colección de opciones (<c>option</c> y <c>optgroup</c>) para la lista desplegable.
        /// </summary>
        /// <returns>Enumeración de <see cref="TagBuilder"/> para cada opción o grupo.</returns>
        private IEnumerable<TagBuilder> BuildOptionHtml()
        {
            //var builder = new StringBuilder();

            // placeholder must be first
            if (Placeholder is not null)
            {
                var option = new TagBuilder("option");
#if !NETCOREAPP && !NETSTANDARD
                option.InnerHtml = HttpUtility.HtmlEncode(Placeholder);
#else
                option.InnerHtml.Append(Placeholder);
#endif
                option.Attributes["value"] = string.Empty;

                //builder.AppendLine(option.ToString(TagRenderMode.Normal));
                yield return option;
            }

            if (DataGroupField.HasValue())
            {
                var groups = BuildGroupList();
                var html = BuildGroupHtml(groups);
                foreach (var option in html)
                    yield return option;
                //builder.Append(html);
            }
            else
            {
                var items = BuildSelectList();
                var html = BuildOptionHtml(items);
                foreach (var option in html)
                    yield return option;
                //builder.Append(html);
            }

            //return builder.ToString();
        }

        /// <summary>
        /// Construye la colección de opciones (<c>option</c>) a partir de una lista de <see cref="SelectListItem"/>.
        /// </summary>
        /// <param name="items">Colección de <see cref="SelectListItem"/>.</param>
        /// <returns>Enumeración de <see cref="TagBuilder"/> para cada opción.</returns>
        private IEnumerable<TagBuilder> BuildOptionHtml(IEnumerable<SelectListItem> items)
        {
            if (items is null)
                yield break;

            foreach (var item in items)
            {
                var option = new TagBuilder("option");
#if !NETCOREAPP && !NETSTANDARD
                option.InnerHtml = HttpUtility.HtmlEncode(item.Text);
#else
                option.InnerHtml.Append(item.Text);
#endif

                if (item.Value is not null)
                    option.Attributes["value"] = item.Value;

                if (item.Selected)
                    option.Attributes["selected"] = "selected";

                yield return option;
                //builder.AppendLine(option.ToString(TagRenderMode.Normal));
            }
        }

        /// <summary>
        /// Construye la colección de grupos (<c>&lt;optgroup&gt;</c>) a partir de una lista de <see cref="SelectGroupItem"/>.
        /// </summary>
        /// <param name="groups">Colección de grupos de selección.</param>
        /// <returns>Enumeración de <see cref="TagBuilder"/> para cada grupo.</returns>
        private IEnumerable<TagBuilder> BuildGroupHtml(IEnumerable<SelectGroupItem> groups)
        {
            if (groups is null)
                yield break;

            foreach (var group in groups)
            {
                var groupTag = new TagBuilder("optgroup");
                groupTag.Attributes["label"] = HttpUtility.HtmlEncode(group.Label);

                var optionsHtml = BuildOptionHtml(group.Items);
#if !NETCOREAPP && !NETSTANDARD
                StringBuilder innerHtmllBuilder = new StringBuilder();
                foreach (var option in optionsHtml)
                    innerHtmllBuilder.AppendLine(option.ToString(TagRenderMode.Normal));
                
                groupTag.InnerHtml = innerHtmllBuilder.ToString();
#else
                foreach (var option in optionsHtml)
                    groupTag.InnerHtml.AppendHtml(option);
#endif

                //builder.AppendLine(groupTag.ToString(TagRenderMode.Normal));
                yield return groupTag;
            }
        }


        /// <summary>
        /// Construye la lista de <see cref="SelectListItem"/> a partir de la colección <see cref="Items"/>.
        /// </summary>
        /// <returns>Enumeración de <see cref="SelectListItem"/>.</returns>
        private IEnumerable<SelectListItem> BuildSelectList()
        {
            if (Items is null)
                return Enumerable.Empty<SelectListItem>();

            if (Items is IEnumerable<SelectListItem> enumerable)
                return enumerable;

            var options = new List<SelectListItem>();

            foreach (var item in Items)
            {
                if (item is null)
                    continue;

                var selectListItem = CreateSelectItem(item);
                options.Add(selectListItem);
            }

            return options;
        }

        /// <summary>
        /// Construye la lista de grupos (<see cref="SelectGroupItem"/>) a partir de la colección <see cref="Items"/>.
        /// </summary>
        /// <returns>Enumeración de <see cref="SelectGroupItem"/>.</returns>
        private IEnumerable<SelectGroupItem> BuildGroupList()
        {
            if (Items is null)
                return Enumerable.Empty<SelectGroupItem>();

            if (Items is IEnumerable<SelectGroupItem> enumerable)
                return enumerable;

            var groups = new Dictionary<string, SelectGroupItem>();

            foreach (var item in Items)
            {
                if (item is null)
                    continue;

                var selectListItem = CreateSelectItem(item);

                var group = LateBinder.GetProperty(item, DataGroupField ?? string.Empty);
                var groupString = Convert.ToString(group) ?? string.Empty; // Ensure groupString is non-null

                var groupItem = groups.GetOrAdd(groupString, k => new SelectGroupItem { Label = k });
                groupItem.Items.Add(selectListItem);
            }

            return groups.Values;
        }

        /// <summary>
        /// Crea un objeto SelectListItem a partir de un objeto
        /// </summary>
        /// <param name="item">Objeto que contiene la información</param>
        /// <returns>Objeto SelectListItem creado</returns>
        private SelectListItem CreateSelectItem(object item)
        {
            var text = DataTextField.HasValue() ? LateBinder.GetProperty(item, DataTextField ?? string.Empty) : item;
            var textString = this.FormatValue(text, DataTextFormat);

            var value = DataValueField.HasValue() ? LateBinder.GetProperty(item, DataValueField ?? string.Empty) : text;
            var valueString = this.FormatValue(value, DataValueFormat);

            var selectedString = this.FormatValue(SelectedValue, DataValueFormat);

            var selected = SelectedValue is not null
                && (Equals(value, SelectedValue) || string.Equals(selectedString, valueString));

            var selectListItem = new SelectListItem
            {
                Text = textString,
                Value = valueString,
                Selected = selected
            };
            return selectListItem;
        }
    }
}