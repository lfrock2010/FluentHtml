using System;
using System.Collections;
using System.Linq.Expressions;
using FluentHtml.Html.Input;
using InputType = FluentHtml.Html.Input.InputType;
using FluentHtml.Fluent;
using System.Linq;

#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace FluentHtml
{
    /// <summary>
    /// Proporciona utilidades para la manipulación de metadatos y etiquetas de controles en componentes HTML.
    /// </summary>
    public static class ElementUtils
    {
#if NETSTANDARD
        /// <summary>
        /// Establece el nombre y la metadata del modelo en un componente que implementa <see cref="IHasModelData"/>, usando una expresión fuertemente tipada.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper{TModel}"/> utilizada para obtener la metadata.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <param name="component">Componente que recibirá el nombre y la metadata.</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="htmlHelper"/> es <c>null</c>.</exception>
        public static void SetMetadata<TModel, TProperty>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IHasModelData component)
        {
            if (htmlHelper is null)
                throw new ArgumentNullException(nameof(htmlHelper));

            component.Name = ExpressionHelper.GetExpressionText(expression);
            var modelExplorer = ModelExplorerUtils.FromLambdaExpression(expression, htmlHelper);
            if (modelExplorer is null)
                return;

            component.Metadata = modelExplorer;     
        }

#elif NETCOREAPP
        /// <summary>
        /// Establece el nombre y la metadata del modelo en un componente que implementa <see cref="IHasModelData"/>, usando una expresión fuertemente tipada.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="htmlHelper">Instancia de <see cref="IHtmlHelper{TModel}"/> utilizada para obtener la metadata.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <param name="component">Componente que recibirá el nombre y la metadata.</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="htmlHelper"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si el servicio <see cref="ModelExpressionProvider"/> no está registrado.</exception>
        public static void SetMetadata<TModel, TProperty>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IHasModelData component)
        {
            if (htmlHelper is null)
                throw new ArgumentNullException(nameof(htmlHelper));

            var expresionProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            component.Name = expresionProvider.GetExpressionText(expression);
            ModelExpression modelExpression = expresionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            if (modelExpression is null || modelExpression.ModelExplorer is null)
                return;

            component.Metadata = modelExpression.ModelExplorer;
        }
#else
        /// <summary>
        /// Establece el nombre y la metadata del modelo en un componente que implementa <see cref="IHasModelData"/>, usando una expresión fuertemente tipada.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="htmlHelper">Instancia de <see cref="HtmlHelper{TModel}"/> utilizada para obtener la metadata.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <param name="component">Componente que recibirá el nombre y la metadata.</param>
        public static void SetMetadata<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IHasModelData component)
        {
            component.Name = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelExplorerUtils.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata is null)
                return;

            component.Metadata = metadata;
        }
#endif

        /// <summary>
        /// Obtiene la etiqueta (label) para un control a partir de la metadata y el texto de la expresión.
        /// </summary>
        /// <param name="metadata">Metadata del modelo asociada al control.</param>
        /// <param name="expressionText">Texto de la expresión que identifica la propiedad.</param>
        /// <returns>
        /// El nombre para mostrar (DisplayName), el nombre de la propiedad, o el último segmento de la expresión.
        /// Si no se encuentra ninguno, retorna una cadena vacía.
        /// </returns>
        public static string GetControlLabel(ModelMetadata? metadata, string? expressionText)
        {
            string? label = null;
            if (metadata is not null)
            {
                label = metadata.DisplayName;
                if (label is null)
                    label = metadata.PropertyName;
            }

            if (label is null && expressionText is not null)
                label = expressionText.Split(new char[] { '.' }).Last<string>();

            return label ?? string.Empty;
        }
    }
}
