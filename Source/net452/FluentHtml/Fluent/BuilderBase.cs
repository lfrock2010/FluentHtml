using System;
using System.Collections.Generic;
using FluentHtml.Extensions;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace FluentHtml.Fluent
{
    public abstract class BuilderBase<TComponent, TBuilder>
        where TComponent : ComponentBase
        where TBuilder : BuilderBase<TComponent, TBuilder>
    {
        protected BuilderBase(TComponent component)
        {
            Component = component;
        }

        public TComponent Component { get; private set; }

        public virtual TBuilder Id(string componentId)
        {
            this.HtmlAttribute("id", TagBuilder.CreateSanitizedId(componentId));
            return this as TBuilder;
        }

        public virtual TBuilder CssStyle(string inlineStyle, object value)
        {
            this.MergeCssStyle(
                new KeyValuePair<string, object>[] { 
                    new KeyValuePair<string, object>(inlineStyle, value) 
                }
            );
            return this as TBuilder;
        }

        public virtual TBuilder CssStyle(object inlineStyles)
        {
            return CssStyle(inlineStyles.ToDictionary());
        }

        public virtual TBuilder CssStyle(IDictionary<string, object> inlineStyles)
        {
            this.MergeCssStyle(inlineStyles);
            return this as TBuilder;
        }

        private void MergeCssStyle(IEnumerable<KeyValuePair<string, object>> inlineStyles)
        {
            Object str;
            List<String> styles = new List<string>();
            if (Component.HtmlAttributes.TryGetValue("style", out str) && !Object.ReferenceEquals(str, null))
                styles.AddRange(str.ToString().Split(';'));

            foreach (KeyValuePair<string, object> valuePair in inlineStyles)
            {
                String inlineStyle = valuePair.Key;
                Object value = valuePair.Value;

                if (string.IsNullOrEmpty(inlineStyle))
                    continue;

                int? index = styles
                                .Where(x => x.Trim().StartsWith(inlineStyle.Trim(), StringComparison.InvariantCultureIgnoreCase))
                                .Select(x => (int?)styles.IndexOf(x))
                                .FirstOrDefault();

                if (index.HasValue && index != -1)
                    styles.RemoveAt(index.Value);

                if (!Object.ReferenceEquals(value, null))
                {
                    string style = string.Format("{0}:{1}", inlineStyle.Trim().ToLower(), value);
                    if (styles.Count > 0)
                        styles.Insert(0, style);
                    else
                        styles.Add(style);
                }
            }

            Component.HtmlAttributes["style"] = String.Join("; ", styles.ToArray());
        }
        
        public virtual TBuilder HtmlAttribute(string name, object value)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if (value != null)
                    Component.HtmlAttributes[name] = this.SerializeAttributeValue(value);
                else
                {
                    if (Component.HtmlAttributes.ContainsKey(name))
                        Component.HtmlAttributes.Remove(name);
                }
            }

            return this as TBuilder;
        }        

        public virtual TBuilder HtmlAttributes(object attributes)
        {
            return HtmlAttributes(attributes.ToDictionary());
        }

        public virtual TBuilder HtmlAttributes(IDictionary<string, object> attributes)
        {
            IEnumerable<KeyValuePair<string, object>> items = attributes
                                                                    .Select(x => new KeyValuePair<string, object>(x.Key, this.SerializeAttributeValue(x.Value)));

            Component.HtmlAttributes.Merge(items);
            return this as TBuilder;
        }

        public virtual TBuilder Data(String index, object value)
        {
            if (!String.IsNullOrEmpty(index) && !String.IsNullOrWhiteSpace(index))
                this.HtmlAttribute("data-" + index, value);

            return this as TBuilder;
        }

        protected String SerializeAttributeValue(Object value)
        {
            if (Object.ReferenceEquals(value, null))
                return null;

            String sObject = null;
            if (!(value is String) && !(value is Guid) && !(value is Guid?) && !(value is DateTime) && !(value is DateTime?))
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
