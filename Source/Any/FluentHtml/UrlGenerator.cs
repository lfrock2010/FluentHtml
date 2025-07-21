using System;
using System.Linq;
using FluentHtml.Extensions;
using FluentHtml.Fluent;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endif

namespace FluentHtml
{
    /// <summary>
    /// Proporciona métodos utilitarios para la generación de URLs en aplicaciones ASP.NET MVC y ASP.NET Core,
    /// soportando rutas, acciones, controladores y URLs relativas o absolutas.
    /// </summary>
    public static class UrlGenerator
    {
#if !NETCOREAPP && !NETSTANDARD
        /// <summary>
        /// Obtiene una instancia de <see cref="UrlHelper"/> para el contexto de MVC clásico.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <returns>Instancia de <see cref="UrlHelper"/>.</returns>
        private static UrlHelper GetUrlHelper(IHtmlHelper helper)
        {
            return new UrlHelper(helper.ViewContext.RequestContext);
        }
#else
        /// <summary>
        /// Obtiene una instancia de <see cref="IUrlHelper"/> para el contexto de ASP.NET Core.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <returns>Instancia de <see cref="IUrlHelper"/>.</returns>
        /// <exception cref="Exception">Si no se puede resolver el servicio necesario para la generación de URLs.</exception>
        private static IUrlHelper GetUrlHelper(IHtmlHelper helper)
        {
            IUrlHelper? urlHelper = (IUrlHelper?)helper.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelper));
            if (urlHelper is null)
            {
                IActionContextAccessor? actionContextAccessor = (IActionContextAccessor?)helper.ViewContext.HttpContext.RequestServices.GetService(typeof(IActionContextAccessor));
                if (actionContextAccessor is null)
                    throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(IActionContextAccessor)));

                IUrlHelperFactory? factory = (IUrlHelperFactory?)helper.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory));
                if (factory is null)
                    throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(IUrlHelperFactory)));

                if (actionContextAccessor.ActionContext is not null)
                    urlHelper = factory.GetUrlHelper(actionContextAccessor.ActionContext);
            }

            if (urlHelper is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(IUrlHelper)));

            return urlHelper;
        }
#endif

        /// <summary>
        /// Genera una URL a partir de una ruta relativa o absoluta.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <param name="url">Ruta relativa o absoluta.</param>
        /// <returns>URL generada como cadena.</returns>
        public static string Generate(IHtmlHelper helper, string url)
        {
            var urlHelper = UrlGenerator.GetUrlHelper(helper);
            return urlHelper.Content(url) ?? string.Empty;
        }

        /// <summary>
        /// Genera una URL basada en un objeto <see cref="NavigationRequest"/> y un diccionario de valores de ruta.
        /// Soporta rutas, acciones/controladores o URLs directas.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <param name="navigationItem">Objeto de navegación con la información de destino.</param>
        /// <param name="routeValues">Valores de ruta adicionales.</param>
        /// <returns>URL generada como cadena.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="helper"/> o <paramref name="navigationItem"/> es <c>null</c>.</exception>
        public static string Generate(IHtmlHelper helper, NavigationRequest navigationItem, RouteValueDictionary routeValues)
        {
            if (helper is null)
                throw new ArgumentNullException(nameof(helper));
            if (navigationItem is null)
                throw new ArgumentNullException(nameof(navigationItem));

            var urlHelper = UrlGenerator.GetUrlHelper(helper);
            string? generatedUrl = null;
            if (!string.IsNullOrEmpty(navigationItem.RouteName))
            {
#if !NETCOREAPP && !NETSTANDARD
                generatedUrl = urlHelper.RouteUrl(navigationItem.RouteName, routeValues);
#else
                generatedUrl = urlHelper.RouteUrl(new UrlRouteContext
                {
                    RouteName = navigationItem.RouteName,
                    Values = routeValues
                });
#endif
            }
            else if (!string.IsNullOrEmpty(navigationItem.ControllerName) && !string.IsNullOrEmpty(navigationItem.ActionName))
            {
#if !NETCOREAPP && !NETSTANDARD
                generatedUrl = urlHelper.Action(navigationItem.ActionName, navigationItem.ControllerName, routeValues, null, null);
#else
                generatedUrl = urlHelper.Action(new UrlActionContext
                {
                    Action = navigationItem.ActionName,
                    Controller = navigationItem.ControllerName,
                    Values = routeValues
                });
#endif
            }
            else if (!string.IsNullOrEmpty(navigationItem.Url) && navigationItem.Url is not null)
            {
                // Si la URL comienza con "~/", se resuelve como recurso relativo al sitio.
                generatedUrl = navigationItem.Url.StartsWith("~/", StringComparison.Ordinal)
                    ? urlHelper.Content(navigationItem.Url)
                    : navigationItem.Url;
            }
            else if (routeValues.Any())
            {
#if !NETCOREAPP && !NETSTANDARD
                generatedUrl = urlHelper.RouteUrl(routeValues);
#else
                generatedUrl = urlHelper.RouteUrl(new UrlRouteContext
                {
                    Values = routeValues
                });
#endif
            }

            return generatedUrl ?? string.Empty;
        }

        /// <summary>
        /// Genera una URL basada en un objeto <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="helper">Instancia de <see cref="IHtmlHelper"/>.</param>
        /// <param name="navigationItem">Objeto de navegación con la información de destino.</param>
        /// <returns>URL generada como cadena.</returns>
        public static string Generate(IHtmlHelper helper, NavigationRequest navigationItem)
        {
            var routeValues = new RouteValueDictionary();

            if (navigationItem.RouteValues.Any())
                routeValues.Merge(navigationItem.RouteValues);

            return Generate(helper, navigationItem, routeValues);
        }
    }
}