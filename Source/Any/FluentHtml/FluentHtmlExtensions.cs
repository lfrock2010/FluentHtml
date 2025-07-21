#if NETCOREAPP || NETSTANDARD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentHtml
{
    /// <summary>
    /// Métodos de extensión para registrar FluentHtml en el contenedor de dependencias de ASP.NET Core.
    /// </summary>
    public static partial class FluentHtmlExtensions
    {
        /// <summary>
        /// Registra los servicios necesarios para FluentHtml en el contenedor de dependencias.
        /// Incluye <see cref="IActionContextAccessor"/> y <see cref="IUrlHelper"/>.
        /// </summary>
        /// <param name="services">La colección de servicios de la aplicación.</param>
        /// <returns>La misma instancia de <see cref="IServiceCollection"/> para encadenamiento.</returns>
        /// <exception cref="System.Exception">
        /// Se lanza si no se puede obtener el <see cref="IActionContextAccessor"/> o el <see cref="ActionContext"/> es nulo.
        /// </exception>
        public static IServiceCollection AddFluentHtml(this IServiceCollection services)
        {
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddScoped<IUrlHelper>(x =>
            {
                IActionContextAccessor? actionContextAccessor = x.GetRequiredService<IActionContextAccessor>();
                if (actionContextAccessor is null)
                    throw new System.Exception(string.Format(Resources.ServiceNotAdded, typeof(IActionContextAccessor)));

                if (actionContextAccessor.ActionContext is null)
                    throw new System.Exception(Resources.InvalidActionContext);

                IUrlHelperFactory? factory = x.GetRequiredService<IUrlHelperFactory>();
                if (factory is null)
                    throw new System.Exception(string.Format(Resources.ServiceNotAdded, typeof(IUrlHelperFactory)));

                return factory.GetUrlHelper(actionContextAccessor.ActionContext);
            });

            return services;
        }
    }
}
#endif