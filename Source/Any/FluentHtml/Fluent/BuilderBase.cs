using System;
using System.Collections.Generic;
using FluentHtml.Extensions;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para los builders de componentes, proporcionando métodos para configurar atributos HTML, estilos CSS y datos personalizados.
    /// </summary>
    /// <typeparam name="TComponent">Tipo del componente que hereda de <see cref="ComponentBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto.</typeparam>
    public abstract class BuilderBase<TComponent, TBuilder>
        where TComponent : ComponentBase
        where TBuilder : BuilderBase<TComponent, TBuilder>
    {
        /// <summary>
        /// Obtiene la instancia del componente asociado a este builder.
        /// </summary>
        public TComponent Component { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="BuilderBase{TComponent, TBuilder}"/>.
        /// </summary>
        /// <param name="component">Componente a asociar con el builder.</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="component"/> es <c>null</c>.</exception>
        protected BuilderBase(TComponent component)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component));
        }

        /// <summary>
        /// Establece el atributo <c>id</c> del componente.
        /// </summary>
        /// <param name="componentId">Identificador a asignar al componente.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder Id(string componentId)
        {
#if !NETCOREAPP && !NETSTANDARD
            this.HtmlAttribute("id", TagBuilder.CreateSanitizedId(componentId));
#else
            this.HtmlAttribute("id", TagBuilder.CreateSanitizedId(componentId, this.Component.HtmlHelper.IdAttributeDotReplacement));
#endif
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Agrega o reemplaza un estilo CSS en línea al componente.
        /// </summary>
        /// <param name="inlineStyle">Nombre de la propiedad CSS.</param>
        /// <param name="value">Valor de la propiedad CSS.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder CssStyle(string inlineStyle, object value)
        {
            this.MergeCssStyle(
                new KeyValuePair<string, object?>[] {
                    new KeyValuePair<string, object?>(inlineStyle, value)
                }
            );
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Agrega o reemplaza varios estilos CSS en línea al componente a partir de un objeto anónimo.
        /// </summary>
        /// <param name="inlineStyles">Objeto con propiedades que representan estilos CSS.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder CssStyle(object inlineStyles)
        {
            return CssStyle(inlineStyles.ToDictionary());
        }

        /// <summary>
        /// Agrega o reemplaza varios estilos CSS en línea al componente a partir de un diccionario.
        /// </summary>
        /// <param name="inlineStyles">Diccionario de estilos CSS.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder CssStyle(IDictionary<string, object?> inlineStyles)
        {
            this.MergeCssStyle(inlineStyles);
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Fusiona los estilos CSS proporcionados con los estilos existentes del componente.
        /// </summary>
        /// <param name="inlineStyles">Colección de pares clave/valor que representan estilos CSS.</param>
        private void MergeCssStyle(IEnumerable<KeyValuePair<string, object?>> inlineStyles)
        {            
            List<string> styles = new List<string>();
            if (Component.HtmlAttributes.TryGetValue("style", out object? str) && str is not null)
                styles.AddRange((str.ToString() ?? string.Empty).Split(';'));

            foreach (KeyValuePair<string, object?> valuePair in inlineStyles)
            {
                string inlineStyle = valuePair.Key;
                object? value = valuePair.Value;

                if (string.IsNullOrEmpty(inlineStyle))
                    continue;

                int? index = styles
                                .Where(x => x.Trim().StartsWith(inlineStyle.Trim(), StringComparison.InvariantCultureIgnoreCase))
                                .Select(x => (int?)styles.IndexOf(x))
                                .FirstOrDefault();

                if (index.HasValue && index != -1)
                    styles.RemoveAt(index.Value);

                if (value is not null)
                {
                    string style = string.Format("{0}:{1}", inlineStyle.Trim().ToLower(), value);
                    if (styles.Count > 0)
                        styles.Insert(0, style);
                    else
                        styles.Add(style);
                }
            }

            Component.HtmlAttributes["style"] = string.Join("; ", styles.ToArray());
        }

        /// <summary>
        /// Establece o elimina un atributo HTML en el componente.
        /// </summary>
        /// <param name="name">Nombre del atributo HTML.</param>
        /// <param name="value">Valor del atributo. Si es <c>null</c>, el atributo se elimina.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder HtmlAttribute(string name, object value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (value is not null)
                    Component.HtmlAttributes[name] = this.SerializeAttributeValue(value);
                else
                {
                    if (Component.HtmlAttributes.ContainsKey(name))
                        Component.HtmlAttributes.Remove(name);
                }
            }

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Agrega o reemplaza múltiples atributos HTML a partir de un objeto anónimo.
        /// </summary>
        /// <param name="attributes">Objeto con propiedades que representan atributos HTML.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder HtmlAttributes(object attributes)
        {
            return HtmlAttributes(attributes.ToDictionary());
        }

        /// <summary>
        /// Agrega o reemplaza múltiples atributos HTML a partir de un diccionario.
        /// </summary>
        /// <param name="attributes">Diccionario de atributos HTML.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder HtmlAttributes(IDictionary<string, object?> attributes)
        {
            IEnumerable<KeyValuePair<string, object?>> items = attributes
                                                                    .Select(x => new KeyValuePair<string, object?>(x.Key, this.SerializeAttributeValue(x.Value)));

            Component.HtmlAttributes.Merge(items);
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Establece un atributo <c>data-*</c> personalizado en el componente.
        /// </summary>
        /// <param name="index">Nombre del atributo de datos (en notación camelCase o PascalCase).</param>
        /// <param name="value">Valor del atributo de datos.</param>
        /// <returns>La instancia actual del builder.</returns>
        public virtual TBuilder Data(string index, object value)
        {
            if (!string.IsNullOrEmpty(index) && !string.IsNullOrWhiteSpace(index))
            {
                StringBuilder key = new StringBuilder();
                foreach (char c in index.ToCharArray())
                {
                    if (char.IsUpper(c))
                    {
                        key.Append("-");
                        key.Append(char.ToLower(c));
                        continue;
                    }

                    key.Append(c);
                }

                this.HtmlAttribute("data-" + key.ToString(), value);
            }

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Serializa el valor de un atributo HTML a una cadena, utilizando JSON para objetos complejos.
        /// </summary>
        /// <param name="value">Valor a serializar.</param>
        /// <returns>Cadena serializada o <c>null</c> si el valor es <c>null</c>.</returns>
        protected string? SerializeAttributeValue(object? value)
        {
            if (value is null)
                return null;

            string? sObject = null;
            if (!(value is string) && !(value is Guid) && !(value is Guid?) && !(value is DateTime) && !(value is DateTime?))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Culture = CultureInfo.InvariantCulture;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                sObject = JsonConvert.SerializeObject(value, settings);
            }
            else
            {
                if (value is String)
                    sObject = Convert.ToString(value, CultureInfo.InvariantCulture);

                if (value is Guid? || value is Guid)
                    sObject = (value as Guid?).GetValueOrDefault().ToString("D");

                if (value is DateTime? || value is DateTime)
                    sObject = (value as DateTime?).GetValueOrDefault().ToString("s");
            }

            return sObject;
        }
    }
}
