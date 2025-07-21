using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentHtml.Extensions;
using FluentHtml.Reflection;

namespace FluentHtml.Html.Input
{
    public class TextBox : InputBase
    {
        public TextBox(HtmlHelper htmlHelper)
            : base(htmlHelper)
        {
            InputType = InputType.Text;
        }

        public override string ToHtmlString()
        {
            VerifySettings();

            string fullName = Name;

            var inputItem = EnumItem<InputType>.Create(InputType);
            string inputType = inputItem.Description.ToLowerInvariant();

            var tagBuilder = new TagBuilder("input");
            MergeAttributes(tagBuilder);
            tagBuilder.MergeAttribute("type", inputType);

            tagBuilder.MergeAttribute("name", fullName, true);
            if (!this.HtmlAttributes.ContainsKey("id"))
                tagBuilder.GenerateId(fullName);

            object value = Value ?? string.Empty;
            string valueParameter = this.FormatValue(value, Format);

            tagBuilder.MergeAttribute("value", valueParameter, true);

            foreach (string cssClass in CssClasses)
                tagBuilder.AddCssClass(cssClass);

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (HtmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);

            ModelMetadata metadata = this.Metadata;
            string propertyName = this.Name;
            if (metadata == null)
                ModelMetadataUtils.FromStringExpression(this.Name, this.ViewContext.ViewData, out metadata, out propertyName);

            IDictionary<string, object> validationAttributes = HtmlHelper.GetUnobtrusiveValidationAttributes(propertyName, metadata, false);
            tagBuilder.MergeAttributes(validationAttributes);

            bool canRead = CanRead();
            bool canWrite = CanWrite();

            if (!canRead)
                tagBuilder.MergeAttribute("type", "password", true);

            if (!canWrite)
                tagBuilder.MergeAttribute("readonly", "readonly", true);

            if ((!canRead || !canWrite) && DeniedClass.HasValue())
                tagBuilder.AddCssClass(DeniedClass);

            if (canRead && canWrite && GrantedClass.HasValue())
                tagBuilder.AddCssClass(GrantedClass);

            return tagBuilder.ToString(TagRenderMode.SelfClosing);
        }

    }
}
