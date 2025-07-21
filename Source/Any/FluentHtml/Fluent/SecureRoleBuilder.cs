using FluentHtml.Extensions;
using System.Collections.Generic;

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Builder fluido para configurar roles y clases CSS de seguridad en componentes derivados de <see cref="SecureViewBase"/>.
    /// Permite asignar roles de lectura/escritura y clases CSS según el acceso concedido o denegado.
    /// </summary>
    public class SecureRoleBuilder
        : BuilderBase<SecureViewBase, SecureRoleBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureRoleBuilder"/>.
        /// </summary>
        /// <param name="component">Componente de vista seguro a asociar con el builder.</param>
        public SecureRoleBuilder(SecureViewBase component)
            : base(component)
        {
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará cuando el acceso esté concedido.
        /// </summary>
        /// <param name="cssClass">Nombre de la clase CSS para acceso concedido.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SecureRoleBuilder GrantedClass(string cssClass)
        {
            Component.GrantedClass = cssClass;
            return this;
        }

        /// <summary>
        /// Establece la clase CSS que se aplicará cuando el acceso esté denegado.
        /// </summary>
        /// <param name="cssClass">Nombre de la clase CSS para acceso denegado.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SecureRoleBuilder DeniedClass(string cssClass)
        {
            Component.DeniedClass = cssClass;
            return this;
        }

        /// <summary>
        /// Agrega uno o más roles de lectura al componente.
        /// </summary>
        /// <param name="roles">Lista de roles de lectura.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SecureRoleBuilder ReadRole(params string[] roles)
        {
            if (roles is not null)
                Component.ReadRoles.AddRange(roles);

            return this;
        }

        /// <summary>
        /// Agrega roles de lectura al componente a partir de un valor en ViewData.
        /// </summary>
        /// <param name="viewDataName">Nombre de la clave en ViewData que contiene los roles.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SecureRoleBuilder ReadRoleViewData(string viewDataName)
        {
            return RoleFromViewData(viewDataName, Component.ReadRoles);
        }

        /// <summary>
        /// Agrega uno o más roles de escritura al componente.
        /// </summary>
        /// <param name="roles">Lista de roles de escritura.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SecureRoleBuilder WriteRole(params string[] roles)
        {
            if (roles is not null)
                Component.WriteRoles.AddRange(roles);

            return this;
        }

        /// <summary>
        /// Agrega roles de escritura al componente a partir de un valor en ViewData.
        /// </summary>
        /// <param name="viewDataName">Nombre de la clave en ViewData que contiene los roles.</param>
        /// <returns>La instancia actual del builder.</returns>
        public SecureRoleBuilder WriteRoleViewData(string viewDataName)
        {
            return RoleFromViewData(viewDataName, Component.WriteRoles);
        }

        /// <summary>
        /// Agrega roles al conjunto especificado a partir de un valor en ViewData.
        /// </summary>
        /// <param name="viewDataName">Nombre de la clave en ViewData que contiene los roles.</param>
        /// <param name="set">Conjunto de roles donde se agregarán los valores encontrados.</param>
        /// <returns>La instancia actual del builder.</returns>
        protected virtual SecureRoleBuilder RoleFromViewData(string viewDataName, ISet<string> set)
        {
            if (!Component.ViewContext.ViewData.TryGetValue(viewDataName, out object? value))
                return this;

            var roles = value as IEnumerable<string>;
            if (roles is null)
                return this;

            set.AddRange(roles);
            return this;
        }
    }
}