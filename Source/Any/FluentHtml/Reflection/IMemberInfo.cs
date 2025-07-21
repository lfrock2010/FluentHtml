using System;
using System.Reflection;

namespace FluentHtml.Reflection
{
    /// <summary>
    /// Define la interfaz para obtener información de miembros (propiedades o campos) de un tipo,
    /// incluyendo su tipo, nombre, metadatos y si dispone de métodos de acceso (getter/setter).
    /// </summary>
    public interface IMemberInfo
    {
        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <value>The type of the member.</value>
        Type MemberType { get; }
        /// <summary>
        /// Gets the member info.
        /// </summary>
        /// <value>The member info.</value>
        MemberInfo MemberInfo { get; }
        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        string Name { get; }
        /// <summary>
        /// Gets a value indicating whether this member has getter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this member has getter; otherwise, <c>false</c>.
        /// </value>
        bool HasGetter { get; }
        /// <summary>
        /// Gets a value indicating whether this member has setter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this member has setter; otherwise, <c>false</c>.
        /// </value>
        bool HasSetter { get; }
    }
}