using System;
using System.Collections;
using System.Linq.Expressions;
using FluentHtml.Html.Input;
using InputType = FluentHtml.Html.Input.InputType;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#else
using System.Web.Mvc;
#endif

namespace FluentHtml
{
    /// <summary>
    /// Métodos de extensión para la creación fluida de controles de entrada HTML (input, textarea, checkbox, radio, etc.) en vistas.
    /// </summary>
    public static class InputExtensions
    {
        /// <summary>
        /// Crea un builder para un control de texto (<c>input type="text"</c>).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="TextBoxBuilder"/>.</returns>
        public static TextBoxBuilder TextBox(this FluentHelper helper)
        {
            var component = new TextBox(helper);
            var builder = new TextBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de texto (<c>input type="text"</c>) enlazado a una expresión de modelo.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad.</param>
        /// <returns>Instancia de <see cref="TextBoxBuilder"/>.</returns>
        public static TextBoxBuilder TextBoxFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new TextBox(helper);

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new TextBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de área de texto (<c>textarea</c>).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="TextAreaBuilder"/>.</returns>
        public static TextAreaBuilder TextArea(this FluentHelper helper)
        {
            var component = new TextArea(helper);
            var builder = new TextAreaBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de área de texto (<c>textarea</c>) enlazado a una expresión de modelo.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad.</param>
        /// <returns>Instancia de <see cref="TextAreaBuilder"/>.</returns>
        public static TextAreaBuilder TextAreaFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new TextArea(helper);
            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new TextAreaBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de contraseña (<c>input type="password"</c>).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="TextBoxBuilder"/>.</returns>
        public static TextBoxBuilder Password(this FluentHelper helper)
        {
            var component = new TextBox(helper) { InputType = InputType.Password };
            var builder = new TextBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de contraseña (<c>input type="password"</c>) enlazado a una expresión de modelo.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad.</param>
        /// <returns>Instancia de <see cref="TextBoxBuilder"/>.</returns>
        public static TextBoxBuilder PasswordFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new TextBox(helper) { InputType = InputType.Password };

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new TextBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control oculto (<c>input type="hidden"</c>).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="TextBoxBuilder"/>.</returns>
        public static TextBoxBuilder Hidden(this FluentHelper helper)
        {
            var component = new TextBox(helper) { InputType = InputType.Hidden };
            var builder = new TextBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control oculto (<c>input type="hidden"</c>) enlazado a una expresión de modelo.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad.</param>
        /// <returns>Instancia de <see cref="TextBoxBuilder"/>.</returns>
        public static TextBoxBuilder HiddenFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new TextBox(helper) { InputType = InputType.Hidden };

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new TextBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de casilla de verificación (<c>input type="checkbox"</c>).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="CheckBoxBuilder"/>.</returns>
        public static CheckBoxBuilder CheckBox(this FluentHelper helper)
        {
            var component = new CheckBox(helper);
            var builder = new CheckBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de casilla de verificación enlazado a una expresión de modelo booleana.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad booleana.</param>
        /// <returns>Instancia de <see cref="CheckBoxBuilder"/>.</returns>
        public static CheckBoxBuilder CheckBoxFor<TModel>(this FluentHelper<TModel> helper, Expression<Func<TModel, bool>> expression)
        {
            var component = new CheckBox(helper);

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            if (component.Value is bool)
                component.Checked = Convert.ToBoolean(component.Value);

            var builder = new CheckBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de casilla de verificación enlazado a una expresión de modelo booleana nullable.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad booleana nullable.</param>
        /// <returns>Instancia de <see cref="CheckBoxBuilder"/>.</returns>
        public static CheckBoxBuilder CheckBoxFor<TModel>(this FluentHelper<TModel> helper, Expression<Func<TModel, bool?>> expression)
        {
            var component = new CheckBox(helper);

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            if (component.Value is bool?)
                component.Checked = Convert.ToBoolean(component.Value);

            var builder = new CheckBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de casilla de verificación tipo slider.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="SliderCheckBoxBuilder"/>.</returns>
        public static SliderCheckBoxBuilder SliderCheckBox(this FluentHelper helper)
        {
            var component = new SliderCheckBox(helper) { IncludeHidden = false };
            var builder = new SliderCheckBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de casilla de verificación tipo slider enlazado a una expresión de modelo booleana.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad booleana.</param>
        /// <returns>Instancia de <see cref="SliderCheckBoxBuilder"/>.</returns>
        public static SliderCheckBoxBuilder SliderCheckBoxFor<TModel>(this FluentHelper<TModel> helper, Expression<Func<TModel, bool>> expression)
        {
            var component = new SliderCheckBox(helper) { IncludeHidden = false };

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            if (component.Value is bool)
                component.Checked = Convert.ToBoolean(component.Value);

            var builder = new SliderCheckBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de casilla de verificación tipo slider enlazado a una expresión de modelo booleana nullable.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad booleana nullable.</param>
        /// <returns>Instancia de <see cref="SliderCheckBoxBuilder"/>.</returns>
        public static SliderCheckBoxBuilder SliderCheckBoxFor<TModel>(this FluentHelper<TModel> helper, Expression<Func<TModel, bool?>> expression)
        {
            var component = new SliderCheckBox(helper) { IncludeHidden = false };

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            if (component.Value is bool?)
                component.Checked = Convert.ToBoolean(component.Value);

            var builder = new SliderCheckBoxBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de botón de opción (<c>input type="radio"</c>).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="RadioButtonBuilder"/>.</returns>
        public static RadioButtonBuilder RadioButton(this FluentHelper helper)
        {
            var component = new RadioButton(helper);
            var builder = new RadioButtonBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para un control de botón de opción enlazado a una expresión de modelo.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad.</param>
        /// <returns>Instancia de <see cref="RadioButtonBuilder"/>.</returns>
        public static RadioButtonBuilder RadioButtonFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new RadioButton(helper);

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new RadioButtonBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para una lista de casillas de verificación (<c>input type="checkbox"</c> múltiple).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="InputListBuilder"/>.</returns>
        public static InputListBuilder CheckBoxList(this FluentHelper helper)
        {
            var component = new InputList(helper)
            {
                InputType = InputType.Checkbox,
                ListCssClass = "checkbox-list"
            };

            var builder = new InputListBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para una lista de casillas de verificación enlazada a una expresión de modelo de tipo lista.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad (debe ser <see cref="IList"/>).</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad de lista.</param>
        /// <returns>Instancia de <see cref="InputListBuilder"/>.</returns>
        public static InputListBuilder CheckBoxListFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
            where TProperty : IList
        {
            var component = new InputList(helper)
            {
                InputType = InputType.Checkbox,
                ListCssClass = "checkbox-list"
            };

#if NETSTANDARD
                component.Name = ExpressionHelper.GetExpressionText(expression);
                var modelExplorer = ModelExplorerUtils.FromLambdaExpression(expression, helper.HtmlHelper);
                if (modelExplorer is not null)
                    component.SelectedValues = modelExplorer.Model as IList;
#elif NETCOREAPP
            var expresionProvider = helper.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            component.Name = expresionProvider.GetExpressionText(expression);

            ModelExpression modelExpression = expresionProvider.CreateModelExpression(helper.HtmlHelper.ViewData, expression);
            if (modelExpression is not null && modelExpression.ModelExplorer is not null)
                component.SelectedValues = modelExpression.Model as IList;
#else
                component.Name = ExpressionHelper.GetExpressionText(expression);
                var metadata = ModelExplorerUtils.FromLambdaExpression(expression, helper.HtmlHelper.ViewData);
                if (metadata is not null)
                    component.SelectedValues = metadata.Model as IList;
#endif

            var builder = new InputListBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para una lista de botones de opción (<c>input type="radio"</c> múltiple).
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/>.</param>
        /// <returns>Instancia de <see cref="InputListBuilder"/>.</returns>
        public static InputListBuilder RadioButtonList(this FluentHelper helper)
        {
            var component = new InputList(helper)
            {
                InputType = InputType.Radio,
                ListCssClass = "radio-list"
            };

            var builder = new InputListBuilder(component);
            return builder;
        }

        /// <summary>
        /// Crea un builder para una lista de botones de opción enlazada a una expresión de modelo de tipo lista.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad (debe ser <see cref="IList"/>).</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/>.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad de lista.</param>
        /// <returns>Instancia de <see cref="InputListBuilder"/>.</returns>
        public static InputListBuilder RadioButtonListFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
            where TProperty : IList
        {
            var component = new InputList(helper)
            {
                InputType = InputType.Radio,
                ListCssClass = "radio-list"
            };

#if NETSTANDARD
                component.Name = ExpressionHelper.GetExpressionText(expression);
                var modelExplorer = ModelExplorerUtils.FromLambdaExpression(expression, helper.HtmlHelper);
                if (modelExplorer is not null)
                    component.SelectedValues = modelExplorer.Model as IList;
#elif NETCOREAPP
            var expresionProvider = helper.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            component.Name = expresionProvider.GetExpressionText(expression);

            ModelExpression modelExpression = expresionProvider.CreateModelExpression(helper.HtmlHelper.ViewData, expression);
            if (modelExpression is not null && modelExpression.ModelExplorer is not null && modelExpression.ModelExplorer.Model is IList list)
                component.SelectedValues = list;
#else
                component.Name = ExpressionHelper.GetExpressionText(expression);
                var metadata = ModelExplorerUtils.FromLambdaExpression(expression, helper.HtmlHelper.ViewData);
                if (metadata is not null && metadata.Model is IList list)
                    component.SelectedValues = list;
#endif

            var builder = new InputListBuilder(component);
            return builder;
        }
    }
}
