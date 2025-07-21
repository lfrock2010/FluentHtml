using System;
using FluentHtml.Extensions;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para builders de componentes que requieren configuración de navegación (acciones, rutas o URLs).
    /// Proporciona métodos para establecer la acción, el controlador, la ruta y la URL de navegación de un componente.
    /// </summary>
    /// <typeparam name="TComponent">Tipo del componente que hereda de <see cref="ComponentBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto.</typeparam>
    public abstract class NavigationBuilderBase<TComponent, TBuilder> : BuilderBase<TComponent, TBuilder>
        where TComponent : ComponentBase
        where TBuilder : NavigationBuilderBase<TComponent, TBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NavigationBuilderBase{TComponent, TBuilder}"/>.
        /// </summary>
        /// <param name="component">Componente a asociar con el builder.</param>
        protected NavigationBuilderBase(TComponent component)
            : base(component)
        {
        }

        /// <summary>
        /// Obtiene la instancia de <see cref="NavigationRequest"/> asociada al componente.
        /// </summary>
        protected abstract NavigationRequest Navigation { get; }

        /// <summary>
        /// Configura la navegación usando acción, controlador y valores de ruta.
        /// </summary>
        /// <param name="actionName">Nombre de la acción.</param>
        /// <param name="controllerName">Nombre del controlador.</param>
        /// <param name="routeValues">Valores de ruta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Action(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            var request = Navigation;
            request.ActionName = actionName;
            request.ControllerName = controllerName;
            request.RouteValues.Merge(routeValues);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Configura la navegación usando acción, controlador y valores de ruta como objeto anónimo.
        /// </summary>
        /// <param name="actionName">Nombre de la acción.</param>
        /// <param name="controllerName">Nombre del controlador.</param>
        /// <param name="routeValues">Valores de ruta como objeto anónimo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Action(string actionName, string controllerName, object? routeValues)
        {
            var request = Navigation;
            request.ActionName = actionName;
            request.ControllerName = controllerName;

            if (routeValues is not null)
                request.RouteValues.Merge(routeValues);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Configura la navegación usando acción y controlador.
        /// </summary>
        /// <param name="actionName">Nombre de la acción.</param>
        /// <param name="controllerName">Nombre del controlador.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, (object?)null);
        }

        /// <summary>
        /// Configura la navegación usando el nombre de la ruta y valores de ruta.
        /// </summary>
        /// <param name="routeName">Nombre de la ruta.</param>
        /// <param name="routeValues">Valores de ruta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Action(string routeName, RouteValueDictionary routeValues)
        {
            var request = Navigation;
            request.RouteName = routeName;
            request.RouteValues.Merge(routeValues);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Configura la navegación usando el nombre de la ruta y valores de ruta como objeto anónimo.
        /// </summary>
        /// <param name="routeName">Nombre de la ruta.</param>
        /// <param name="routeValues">Valores de ruta como objeto anónimo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Action(string routeName, object? routeValues)
        {
            var request = Navigation;
            request.RouteName = routeName;
            if (routeValues is not null)
                request.RouteValues.Merge(routeValues);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Configura la navegación usando solo el nombre de la ruta.
        /// </summary>
        /// <param name="routeName">Nombre de la ruta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Action(string routeName)
        {
            return Action(routeName, (object?)null);
        }

        /// <summary>
        /// Establece la URL directa de navegación del componente.
        /// </summary>
        /// <param name="url">URL a asignar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Url(string url)
        {
            Navigation.Url = url;
            return (this as TBuilder)!;
        }

    }
}