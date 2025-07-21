using System;
using System.Collections.Generic;
using FluentHtml.Extensions;

namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para builders de componentes seguros (<see cref="SecureViewBase"/>),
    /// proporcionando métodos para configurar roles de seguridad y clases CSS asociadas a permisos.
    /// </summary>
    /// <typeparam name="TView">Tipo del componente de vista que hereda de <see cref="SecureViewBase"/>.</typeparam>
    /// <typeparam name="TBuilder">Tipo del builder concreto.</typeparam>
    public abstract class SecureViewBuilderBase<TView, TBuilder>
        : ViewComponentBuilderBase<TView, TBuilder>
        where TView : SecureViewBase
        where TBuilder : SecureViewBuilderBase<TView, TBuilder>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SecureViewBuilderBase{TView, TBuilder}"/>.
        /// </summary>
        /// <param name="component">Componente seguro a asociar con el builder.</param>
        protected SecureViewBuilderBase(TView component)
            : base(component)
        {
        }

        /// <summary>
        /// Asigna uno o más roles de escritura al componente para controlar el acceso.
        /// </summary>
        /// <param name="writeRoles">Lista de roles de escritura permitidos.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Secure(params string[] writeRoles)
        {
            Component.WriteRoles.AddRange(writeRoles);
            return (this as TBuilder)!;
        }

        /// <summary>
        /// Asigna una colección de roles de escritura al componente para controlar el acceso.
        /// </summary>
        /// <param name="writeRoles">Colección de roles de escritura permitidos.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Secure(IEnumerable<string> writeRoles)
        {
            if (writeRoles is not null)
                Component.WriteRoles.AddRange(writeRoles);

            return (this as TBuilder)!;
        }

        /// <summary>
        /// Permite configurar roles y clases CSS de seguridad mediante un objeto <see cref="SecureRoleBuilder"/>.
        /// </summary>
        /// <param name="configurator">Acción de configuración que recibe un <see cref="SecureRoleBuilder"/>.</param>
        /// <returns>La instancia actual del builder.</returns>
        public TBuilder Secure(Action<SecureRoleBuilder> configurator)
        {
            var builder = new SecureRoleBuilder(Component);
            configurator(builder);

            return (this as TBuilder)!;
        }
    }
}