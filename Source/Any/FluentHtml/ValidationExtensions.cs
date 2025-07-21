using System;
using System.Linq.Expressions;
using FluentHtml.Html.Validation;

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
    /// Proporciona métodos de extensión para generar mensajes e iconos de validación en vistas utilizando FluentHtml.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Genera un builder para un icono de validación asociado al contexto actual.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/> utilizada para la generación del componente.</param>
        /// <returns>Instancia de <see cref="ValidationMessageBuilder"/> configurada para mostrar solo el icono de validación.</returns>
        public static ValidationMessageBuilder ValidationImage(this FluentHelper helper)
        {
            var component = new ValidationMessage(helper) { IconOnly = true };

            var builder = new ValidationMessageBuilder(component);
            return builder;
        }

        /// <summary>
        /// Genera un builder para un icono de validación asociado a una expresión de modelo fuertemente tipada.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/> utilizada para la generación del componente.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <returns>Instancia de <see cref="ValidationMessageBuilder"/> configurada para mostrar solo el icono de validación.</returns>
        /// <exception cref="Exception">Se lanza si el proveedor de expresiones de modelo no está registrado (solo en .NET Core).</exception>
        public static ValidationMessageBuilder ValidationImageFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new ValidationMessage(helper) { IconOnly = true };

#if NETSTANDARD
                component.ModelName = ExpressionHelper.GetExpressionText(expression);
#elif NETCOREAPP
            var expresionProvider = helper.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            component.ModelName = expresionProvider.GetExpressionText(expression);
#else
                component.ModelName = ExpressionHelper.GetExpressionText(expression);
#endif

            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);
            var builder = new ValidationMessageBuilder(component);
            return builder;
        }

        /// <summary>
        /// Genera un builder para un mensaje de validación asociado al contexto actual.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="FluentHelper"/> utilizada para la generación del componente.</param>
        /// <returns>Instancia de <see cref="ValidationMessageBuilder"/> para configurar y renderizar el mensaje de validación.</returns>
        public static ValidationMessageBuilder ValidationMessage(this FluentHelper helper)
        {
            var component = new ValidationMessage(helper);

            var builder = new ValidationMessageBuilder(component);
            return builder;
        }

        /// <summary>
        /// Genera un builder para un mensaje de validación asociado a una expresión de modelo fuertemente tipada.
        /// </summary>
        /// <typeparam name="TModel">Tipo del modelo.</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad.</typeparam>
        /// <param name="helper">Instancia de <see cref="FluentHelper{TModel}"/> utilizada para la generación del componente.</param>
        /// <param name="expression">Expresión lambda que identifica la propiedad del modelo.</param>
        /// <returns>Instancia de <see cref="ValidationMessageBuilder"/> para configurar y renderizar el mensaje de validación.</returns>
        /// <exception cref="Exception">Se lanza si el proveedor de expresiones de modelo no está registrado (solo en .NET Core).</exception>
        public static ValidationMessageBuilder ValidationMessageFor<TModel, TProperty>(this FluentHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var component = new ValidationMessage(helper);

#if NETSTANDARD
                component.ModelName = ExpressionHelper.GetExpressionText(expression);
#elif NETCOREAPP
            var expresionProvider = helper.HtmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            component.ModelName = expresionProvider.GetExpressionText(expression);
#else
                component.ModelName = ExpressionHelper.GetExpressionText(expression);
#endif
            ElementUtils.SetMetadata(helper.HtmlHelper, expression, component);

            var builder = new ValidationMessageBuilder(component);
            return builder;
        }

    }
}
