#if !NETCOREAPP && !NETSTANDARD
using System.Reflection;
namespace System.Web.Mvc
{
    /// <summary>
    /// Métodos de extensión para <see cref="ViewContext"/> relacionados con la validación del cliente.
    /// </summary>
    public static class ViewContextExtensions
    {
        /// <summary>
        /// Obtiene el <see cref="FormContext"/> asociado a la validación del cliente para el contexto de vista actual.
        /// Utiliza reflexión para acceder a métodos internos si es necesario.
        /// </summary>
        /// <param name="viewContext">El contexto de la vista.</param>
        /// <returns>
        /// Instancia de <see cref="FormContext"/> si la validación del cliente está habilitada; de lo contrario, <c>null</c>.
        /// </returns>
        public static FormContext? GetFormContextForClientValidation(this ViewContext viewContext)
        {
            if (viewContext is null)
                return null;

            MethodInfo? methodInfo = typeof(ViewContext).GetMethod("GetFormContextForClientValidation", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo is not null)
                return (FormContext)methodInfo.Invoke(viewContext, new object[] { });

            if (!viewContext.ClientValidationEnabled)
                return null;

            return viewContext.FormContext;
        }
    }
}
#endif
