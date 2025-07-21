using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentHtml.Reflection
{
    /// <summary>
    /// Proporciona utilidades para operaciones de enlace tardío (late binding) sobre tipos .NET,
    /// permitiendo invocar métodos, acceder y modificar propiedades o campos en tiempo de ejecución.
    /// </summary>
    public static class LateBinder
    {
        /// <summary>
        /// <see cref="BindingFlags"/> predeterminados para miembros públicos de instancia.
        /// </summary>
        public const BindingFlags DefaultPublicFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        /// <summary>
        /// <see cref="BindingFlags"/> predeterminados para miembros públicos y no públicos de instancia.
        /// </summary>
        public const BindingFlags DefaultNonPublicFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        /// <inheritdoc cref="FindMethod(Type, string, object[])"/>
        public static IMethodAccessor? FindMethod(Type type, string name, params object[] arguments)
        {
            return FindMethod(type, name, DefaultPublicFlags, arguments);
        }

        /// <summary>
        /// Busca el método especificado por nombre y argumentos, usando los <see cref="BindingFlags"/> indicados.
        /// </summary>
        /// <param name="type">Tipo donde buscar el método.</param>
        /// <param name="name">Nombre del método.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <param name="arguments">Argumentos para inferir la sobrecarga.</param>
        /// <returns>Instancia de <see cref="IMethodAccessor"/> si se encuentra; si no, <c>null</c>.</returns>
        public static IMethodAccessor? FindMethod(Type type, string name, BindingFlags flags, params object[] arguments)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            TypeAccessor typeAccessor = TypeAccessor.GetAccessor(type);

            Type[] types = arguments
              .Select(a => a is null ? typeof(object) : a.GetType())
              .ToArray();

            var methodAccessor = typeAccessor.FindMethod(name, types, flags);

            return methodAccessor;
        }

        /// <summary>
        /// Busca una propiedad a partir de una expresión lambda.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto que contiene la propiedad.</typeparam>
        /// <param name="propertyExpression">Expresión lambda de la propiedad (por ejemplo, <c>p =&gt; p.Propiedad</c>).</param>
        /// <returns>Instancia de <see cref="IMemberAccessor"/> si se encuentra; si no, <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="propertyExpression"/> es <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Si la expresión no es válida o la propiedad es estática.</exception>
        public static IMemberAccessor? FindProperty<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            TypeAccessor typeAccessor = TypeAccessor.GetAccessor(typeof(T));
            return typeAccessor.FindProperty<T>(propertyExpression);
        }

        /// <inheritdoc cref="FindProperty(Type, string, BindingFlags)"/>
        public static IMemberAccessor? FindProperty(Type type, string name)
        {
            return FindProperty(type, name, DefaultPublicFlags);
        }

        /// <summary>
        /// Busca una propiedad por nombre, usando los <see cref="BindingFlags"/> indicados.
        /// Soporta nombres anidados (por ejemplo, <c>Persona.Direccion.CodigoPostal</c>).
        /// </summary>
        /// <param name="type">Tipo donde buscar la propiedad.</param>
        /// <param name="name">Nombre de la propiedad (puede ser anidado).</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <returns>Instancia de <see cref="IMemberAccessor"/> si se encuentra; si no, <c>null</c>.</returns>
        public static IMemberAccessor? FindProperty(Type type, string name, BindingFlags flags)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type currentType = type;
            TypeAccessor typeAccessor;
            IMemberAccessor? memberAccessor = null;

            // Soporta propiedades anidadas
            var parts = name.Split('.');
            foreach (var part in parts)
            {
                if (memberAccessor is not null)
                    currentType = memberAccessor.MemberType;

                typeAccessor = TypeAccessor.GetAccessor(currentType);
                memberAccessor = typeAccessor.FindProperty(part, flags);
            }

            return memberAccessor;
        }

        /// <inheritdoc cref="FindField(Type, string, BindingFlags)"/>
        public static IMemberAccessor? FindField(Type type, string name)
        {
            return FindField(type, name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Busca un campo por nombre, usando los <see cref="BindingFlags"/> indicados.
        /// </summary>
        /// <param name="type">Tipo donde buscar el campo.</param>
        /// <param name="name">Nombre del campo.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <returns>Instancia de <see cref="IMemberAccessor"/> si se encuentra; si no, <c>null</c>.</returns>
        public static IMemberAccessor? FindField(Type type, string name, BindingFlags flags)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            TypeAccessor typeAccessor = TypeAccessor.GetAccessor(type);
            IMemberAccessor? memberAccessor = typeAccessor.FindField(name, flags);

            return memberAccessor;
        }

        /// <inheritdoc cref="Find(Type, string, BindingFlags)"/>
        public static IMemberAccessor? Find(Type type, string name)
        {
            return Find(type, name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Busca una propiedad o campo por nombre, usando los <see cref="BindingFlags"/> indicados.
        /// </summary>
        /// <param name="type">Tipo donde buscar.</param>
        /// <param name="name">Nombre de la propiedad o campo.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <returns>Instancia de <see cref="IMemberAccessor"/> si se encuentra; si no, <c>null</c>.</returns>
        public static IMemberAccessor? Find(Type type, string name, BindingFlags flags)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            TypeAccessor typeAccessor = TypeAccessor.GetAccessor(type);
            IMemberAccessor? memberAccessor = typeAccessor.Find(name, flags);

            return memberAccessor;
        }

        /// <inheritdoc cref="SetProperty(object, string, object, BindingFlags)"/>
        public static void SetProperty(object target, string name, object value)
        {
            SetProperty(target, name, value, DefaultPublicFlags);
        }

        /// <summary>
        /// Establece el valor de una propiedad, soportando nombres anidados.
        /// </summary>
        /// <param name="target">Objeto destino.</param>
        /// <param name="name">Nombre de la propiedad (puede ser anidado).</param>
        /// <param name="value">Nuevo valor.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        public static void SetProperty(object target, string name, object value, BindingFlags flags)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            Type currentType = rootType;
            object currentTarget = target;

            TypeAccessor typeAccessor;
            IMemberAccessor? memberAccessor = null;

            // Soporta propiedades anidadas
            var parts = name.Split('.');
            foreach (var part in parts)
            {
                if (memberAccessor is not null)
                {
                    currentTarget = memberAccessor.GetValue(currentTarget);
                    currentType = memberAccessor.MemberType;
                }

                typeAccessor = TypeAccessor.GetAccessor(currentType);
                memberAccessor = typeAccessor.FindProperty(part, flags);
            }

            if (memberAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find property '{0}' in type '{1}'.", name, rootType.Name));

            memberAccessor.SetValue(currentTarget, value);
        }

        /// <inheritdoc cref="SetField(object, string, object, BindingFlags)"/>
        public static void SetField(object target, string name, object value)
        {
            SetField(target, name, value, DefaultPublicFlags);
        }

        /// <summary>
        /// Establece el valor de un campo.
        /// </summary>
        /// <param name="target">Objeto destino.</param>
        /// <param name="name">Nombre del campo.</param>
        /// <param name="value">Nuevo valor.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        public static void SetField(object target, string name, object value, BindingFlags flags)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            var memberAccessor = FindField(rootType, name, flags);

            if (memberAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find field '{0}' in type '{1}'.", name, rootType.Name));

            memberAccessor.SetValue(target, value);
        }

        /// <inheritdoc cref="Set(object, string, object, BindingFlags)"/>
        public static void Set(object target, string name, object value)
        {
            Set(target, name, value, DefaultPublicFlags);
        }

        /// <summary>
        /// Establece el valor de una propiedad o campo.
        /// </summary>
        /// <param name="target">Objeto destino.</param>
        /// <param name="name">Nombre de la propiedad o campo.</param>
        /// <param name="value">Nuevo valor.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        public static void Set(object target, string name, object value, BindingFlags flags)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            var memberAccessor = Find(rootType, name, flags);

            if (memberAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find a property or field with a name of '{0}' in type '{1}'.", name, rootType.Name));

            memberAccessor.SetValue(target, value);
        }

        /// <inheritdoc cref="GetProperty(object, string, BindingFlags)"/>
        public static object GetProperty(object target, string name)
        {
            return GetProperty(target, name, DefaultPublicFlags);
        }

        /// <summary>
        /// Obtiene el valor de una propiedad, soportando nombres anidados.
        /// </summary>
        /// <param name="target">Objeto origen.</param>
        /// <param name="name">Nombre de la propiedad (puede ser anidado).</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <returns>Valor de la propiedad.</returns>
        public static object GetProperty(object target, string name, BindingFlags flags)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            Type currentType = rootType;
            object currentTarget = target;

            IMemberAccessor? memberAccessor = null;

            // Soporta propiedades anidadas
            var parts = name.Split('.');
            foreach (var part in parts)
            {
                if (memberAccessor is not null)
                {
                    currentTarget = memberAccessor.GetValue(currentTarget);
                    currentType = memberAccessor.MemberType;
                }

                var typeAccessor = TypeAccessor.GetAccessor(currentType);
                memberAccessor = typeAccessor.FindProperty(part, flags);
            }

            if (memberAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find property '{0}' in type '{1}'.", name, rootType.Name));

            return memberAccessor.GetValue(currentTarget);
        }

        /// <inheritdoc cref="GetField(object, string, BindingFlags)"/>
        public static object GetField(object target, string name)
        {
            return GetField(target, name, DefaultPublicFlags);
        }

        /// <summary>
        /// Obtiene el valor de un campo.
        /// </summary>
        /// <param name="target">Objeto origen.</param>
        /// <param name="name">Nombre del campo.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <returns>Valor del campo.</returns>
        public static object GetField(object target, string name, BindingFlags flags)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            var memberAccessor = FindField(rootType, name, flags);
            if (memberAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find field '{0}' in type '{1}'.", name, rootType.Name));

            return memberAccessor.GetValue(target);
        }

        /// <inheritdoc cref="Get(object, string, BindingFlags)"/>
        public static object Get(object target, string name)
        {
            return Get(target, name, DefaultPublicFlags);
        }

        /// <summary>
        /// Obtiene el valor de una propiedad o campo.
        /// </summary>
        /// <param name="target">Objeto origen.</param>
        /// <param name="name">Nombre de la propiedad o campo.</param>
        /// <param name="flags">Máscara de <see cref="BindingFlags"/> para la búsqueda.</param>
        /// <returns>Valor de la propiedad o campo.</returns>
        public static object Get(object target, string name, BindingFlags flags)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            var memberAccessor = Find(rootType, name, flags);
            if (memberAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find a property or field with a name of '{0}' in type '{1}'.", name, rootType.Name));

            return memberAccessor.GetValue(target);
        }

        /// <summary>
        /// Crea una instancia del tipo especificado usando el constructor predeterminado.
        /// </summary>
        /// <param name="type">Tipo a instanciar.</param>
        /// <returns>Nueva instancia del tipo.</returns>
        public static object CreateInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var typeAccessor = TypeAccessor.GetAccessor(type);
            if (typeAccessor is null)
                throw new InvalidOperationException(string.Format("Could not find constructor for {0}.", type.Name));

            return typeAccessor.Create();
        }

        /// <summary>
        /// Invoca un método por nombre sobre el objeto especificado.
        /// </summary>
        /// <param name="target">Objeto sobre el que se invocará el método.</param>
        /// <param name="name">Nombre del método.</param>
        /// <param name="arguments">Argumentos para el método.</param>
        /// <returns>Valor de retorno del método invocado.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="target"/> o <paramref name="name"/> es <c>null</c> o vacío.</exception>
        /// <exception cref="InvalidOperationException">Si el método no se encuentra.</exception>
        public static object InvokeMethod(object target, string name, params object[] arguments)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type rootType = target.GetType();
            var methodAccessor = FindMethod(rootType, name);

            if (methodAccessor is null)
                throw new InvalidOperationException(string.Format(
                    "Could not find method '{0}' in type '{1}'.", name, rootType.Name));

            return methodAccessor.Invoke(target, arguments);

        }
    }
}
