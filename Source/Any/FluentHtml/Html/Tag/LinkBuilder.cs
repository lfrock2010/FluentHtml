using System;
using FluentHtml.Extensions;
using FluentHtml.Fluent;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace FluentHtml.Html.Tag
{
    /// <summary>
    /// Builder fluido para configurar y personalizar un componente <see cref="Link"/>.
    /// Permite establecer la navegación, el texto, el HTML, el destino, la codificación y atributos adicionales del enlace.
    /// </summary>
    public class LinkBuilder : SecureViewBuilderBase<Link, LinkBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="LinkBuilder"/>.
        /// </summary>
        /// <param name="component">Componente <see cref="Link"/> a asociar con el builder.</param>
        public LinkBuilder(Link component)
            : base(component)
        {
        }

        /// <summary>
        /// Configura la navegación del enlace usando acción, controlador y valores de ruta.
        /// </summary>
        /// <param name="actionName">Nombre de la acción.</param>
        /// <param name="controllerName">Nombre del controlador.</param>
        /// <param name="routeValues">Valores de ruta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Action(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            var request = Component.Navigation;
            request.ActionName = actionName;
            request.ControllerName = controllerName;
            request.RouteValues.Merge(routeValues);

            return this;
        }

        /// <summary>
        /// Configura la navegación del enlace usando acción, controlador y valores de ruta como objeto.
        /// </summary>
        /// <param name="actionName">Nombre de la acción.</param>
        /// <param name="controllerName">Nombre del controlador.</param>
        /// <param name="routeValues">Valores de ruta como objeto anónimo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Action(string actionName, string controllerName, object? routeValues)
        {
            var request = Component.Navigation;
            request.ActionName = actionName;
            request.ControllerName = controllerName;

            if (routeValues is not null)
                request.RouteValues.Merge(routeValues);

            return this;
        }

        /// <summary>
        /// Configura la navegación del enlace usando acción y controlador.
        /// </summary>
        /// <param name="actionName">Nombre de la acción.</param>
        /// <param name="controllerName">Nombre del controlador.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, (object?)null);
        }

        /// <summary>
        /// Configura la navegación del enlace usando el nombre de la ruta y valores de ruta.
        /// </summary>
        /// <param name="routeName">Nombre de la ruta.</param>
        /// <param name="routeValues">Valores de ruta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Action(string routeName, RouteValueDictionary routeValues)
        {
            var request = Component.Navigation;
            request.RouteName = routeName;
            request.RouteValues.Merge(routeValues);

            return this;
        }

        /// <summary>
        /// Configura la navegación del enlace usando el nombre de la ruta y valores de ruta como objeto.
        /// </summary>
        /// <param name="routeName">Nombre de la ruta.</param>
        /// <param name="routeValues">Valores de ruta como objeto anónimo.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Action(string routeName, object? routeValues)
        {
            var request = Component.Navigation;
            request.RouteName = routeName;

            if (routeValues is not null)
                request.RouteValues.Merge(routeValues);

            return this;
        }

        /// <summary>
        /// Configura la navegación del enlace usando solo el nombre de la ruta.
        /// </summary>
        /// <param name="routeName">Nombre de la ruta.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Action(string routeName)
        {
            return Action(routeName, (object?)null);
        }

        /// <summary>
        /// Establece la URL directa del enlace.
        /// </summary>
        /// <param name="url">URL a asignar al enlace.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Url(string url)
        {
            Component.Navigation.Url = url;
            return this;
        }

        /// <summary>
        /// Establece si el texto del enlace debe ser codificado como HTML.
        /// </summary>
        /// <param name="value">Valor que indica si se debe codificar el texto (por defecto <c>true</c>).</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Encode(bool value = true)
        {
            Component.Encode = value;
            return this;
        }

        /// <summary>
        /// Establece el texto que se mostrará dentro del enlace.
        /// </summary>
        /// <param name="text">Texto a mostrar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Text(string text)
        {
            Component.Text = text;
            return this;
        }

        /// <summary>
        /// Establece el contenido HTML que se mostrará dentro del enlace (sin codificar).
        /// </summary>
        /// <param name="html">HTML a mostrar.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Html(string html)
        {
            Component.Encode = false;
            Component.Text = html;
            return this;
        }

        /// <summary>
        /// Establece el atributo <c>target</c> del enlace (por ejemplo, <c>_blank</c>).
        /// </summary>
        /// <param name="target">Valor del atributo target.</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Target(string target)
        {
            const string name = "target";

            if (target.HasValue())
                HtmlAttribute(name, target);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }

        /// <summary>
        /// Establece o elimina el atributo <c>disabled</c> en el enlace.
        /// </summary>
        /// <param name="disabled">Indica si el enlace debe estar deshabilitado (por defecto <c>true</c>).</param>
        /// <returns>La instancia actual del builder.</returns>
        public LinkBuilder Disabled(bool disabled = true)
        {
            const string name = "disabled";

            if (disabled)
                HtmlAttribute(name, name);
            else
                Component.HtmlAttributes.Remove(name);

            return this;
        }
    }
}