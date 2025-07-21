using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using FluentHtml.Extensions;

namespace FluentHtml.Reflection
{
    /// <summary>
    /// Proporciona utilidades de reflexión para la extracción de nombres y metadatos de propiedades, coerción de valores y análisis de tipos.
    /// </summary>
    public static class ReflectionHelper
    {
        private static readonly Type _stringType = typeof(string);
        private static readonly Type _byteArrayType = typeof(byte[]);
        private static readonly Type _nullableType = typeof(Nullable<>);
        private static readonly Type _genericCollectionType = typeof(ICollection<>);
        private static readonly Type _collectionType = typeof(ICollection);
        private static readonly Type _genericDictionaryType = typeof(IDictionary<,>);
        private static readonly Type _dictionaryType = typeof(IDictionary);

        /// <inheritdoc cref="ExtractPropertyName{TValue}(Expression{Func{TValue}})"/>
        public static string? ExtractPropertyName<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Body is not MemberExpression memberExpression)
                return null;

            return ExtractPropertyName(memberExpression);
        }

        /// <inheritdoc cref="ExtractPropertyName{TSource, TValue}(Expression{Func{TSource, TValue}})"/>
        public static string? ExtractPropertyName<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Body is not MemberExpression memberExpression)
                return null;

            return ExtractPropertyName(memberExpression);
        }

        /// <summary>
        /// Extrae el nombre de la propiedad a partir de una expresión de miembro.
        /// </summary>
        /// <param name="memberExpression">Expresión de miembro.</param>
        /// <returns>Nombre de la propiedad.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="memberExpression"/> es <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Si la expresión no representa una propiedad o es estática.</exception>
        public static string ExtractPropertyName(MemberExpression memberExpression)
        {
            var property = ExtractPropertyInfo(memberExpression);

            var getMethod = property.GetGetMethod(true);
            if (getMethod is not null && getMethod.IsStatic)
                throw new ArgumentException("The referenced property is a static property.", "memberExpression");

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Extrae el nombre de la columna (o propiedad) a partir de una expresión de propiedad.
        /// </summary>
        /// <typeparam name="TValue">Tipo del valor de la propiedad.</typeparam>
        /// <param name="propertyExpression">Expresión de propiedad (por ejemplo, p =&gt; p.Propiedad).</param>
        /// <returns>Nombre de la columna o <c>null</c> si no es una expresión de miembro.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="propertyExpression"/> es <c>null</c>.</exception>
        public static string? ExtractColumnName<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Body is not MemberExpression memberExpression)
                return null;

            return ExtractColumnName(memberExpression);
        }

        /// <summary>
        /// Extrae el nombre de la columna (o propiedad) a partir de una expresión de propiedad.
        /// </summary>
        /// <typeparam name="TSource">Tipo del objeto origen.</typeparam>
        /// <typeparam name="TValue">Tipo del valor de la propiedad.</typeparam>
        /// <param name="propertyExpression">Expresión de propiedad (por ejemplo, p =&gt; p.Propiedad).</param>
        /// <returns>Nombre de la columna o <c>null</c> si no es una expresión de miembro.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="propertyExpression"/> es <c>null</c>.</exception>
        public static string? ExtractColumnName<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Body is not MemberExpression memberExpression)
                return null;

            return ExtractColumnName(memberExpression);
        }

        /// <summary>
        /// Extrae el nombre de la columna (o propiedad) a partir de una expresión de miembro, considerando atributos de nombre de columna.
        /// </summary>
        /// <param name="memberExpression">Expresión de miembro.</param>
        /// <returns>Nombre de la columna.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="memberExpression"/> es <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Si la expresión no representa una propiedad o es estática.</exception>
        public static string ExtractColumnName(MemberExpression memberExpression)
        {
            var property = ExtractPropertyInfo(memberExpression);

            var getMethod = property.GetGetMethod(true);
            if (getMethod is not null && getMethod.IsStatic)
                throw new ArgumentException("The referenced property is a static property.", "memberExpression");

            string columnName = property.Name;

#if !NET40
            var display = Attribute.GetCustomAttribute(property, typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)) as System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;
            if (display is not null && display.Name!.HasValue())
                columnName = display.Name!;
#else
            var display = Attribute.GetCustomAttribute(property, typeof(System.ComponentModel.DataAnnotations.DisplayAttribute)) as System.ComponentModel.DataAnnotations.DisplayAttribute;
            if (display is not null && display.Name.HasValue())
                columnName = !display.Name;
#endif

            return columnName;
        }

        /// <summary>
        /// Extrae la información de la propiedad a partir de una expresión de propiedad.
        /// </summary>
        /// <typeparam name="TValue">Tipo del valor de la propiedad.</typeparam>
        /// <param name="propertyExpression">Expresión de propiedad.</param>
        /// <returns>Instancia de <see cref="PropertyInfo"/> o <c>null</c> si no es una expresión de miembro.</returns>
        public static PropertyInfo? ExtractPropertyInfo<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Body is not MemberExpression memberExpression)
                return null;

            return ExtractPropertyInfo(memberExpression);
        }

        /// <summary>
        /// Extrae la información de la propiedad a partir de una expresión de propiedad.
        /// </summary>
        /// <typeparam name="TSource">Tipo del objeto origen.</typeparam>
        /// <typeparam name="TValue">Tipo del valor de la propiedad.</typeparam>
        /// <param name="propertyExpression">Expresión de propiedad.</param>
        /// <returns>Instancia de <see cref="PropertyInfo"/> o <c>null</c> si no es una expresión de miembro.</returns>
        public static PropertyInfo? ExtractPropertyInfo<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            if (propertyExpression is null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression.Body is not MemberExpression memberExpression)
                return null;

            return ExtractPropertyInfo(memberExpression);
        }

        /// <summary>
        /// Extrae la información de la propiedad a partir de una expresión de miembro.
        /// </summary>
        /// <param name="memberExpression">Expresión de miembro.</param>
        /// <returns>Instancia de <see cref="PropertyInfo"/>.</returns>
        /// <exception cref="ArgumentException">Si la expresión no es de acceso a miembro o no accede a una propiedad.</exception>
        public static PropertyInfo ExtractPropertyInfo(MemberExpression memberExpression)
        {
            if (memberExpression is null)
                throw new ArgumentException("The expression is not a member access expression.", "memberExpression");

            var property = memberExpression.Member as PropertyInfo;
            if (property is null)
                throw new ArgumentException("The member access expression does not access a property.", "memberExpression");

            return property;
        }

        /// <summary>
        /// Obtiene el tipo subyacente, considerando <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="type">Tipo a analizar.</param>
        /// <returns>Tipo subyacente si es nullable; de lo contrario, el mismo tipo.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            Type t = type;
            bool isNullable = t.IsGenericType && (t.GetGenericTypeDefinition() == _nullableType);
            if (isNullable)
                return Nullable.GetUnderlyingType(t) ?? t;

            return t;
        }

        /// <summary>
        /// Determina si el tipo especificado es una colección.
        /// </summary>
        /// <param name="type">Tipo a comprobar.</param>
        /// <returns><c>true</c> si es colección; en caso contrario, <c>false</c>.</returns>
        public static bool IsCollection(this Type type)
        {
            return type.GetInterfaces()
                .Union(new[] { type })
                .Any(x => x == typeof(ICollection) || (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)));
        }

        /// <summary>
        /// Determina si el tipo especificado es una colección y obtiene el tipo de elemento.
        /// </summary>
        /// <param name="type">Tipo a comprobar.</param>
        /// <param name="elementType">Tipo de elemento de la colección.</param>
        /// <returns><c>true</c> si es colección; en caso contrario, <c>false</c>.</returns>
        public static bool IsCollection(this Type type, out Type elementType)
        {
            elementType = type;
            var collectionType = type
                .GetInterfaces()
                .Union(new[] { type })
                .FirstOrDefault(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(ICollection<>)));

            if (collectionType is null)
                return false;

            elementType = collectionType.GetGenericArguments().Single();
            return true;
        }

        /// <summary>
        /// Determina si el tipo especificado es un diccionario.
        /// </summary>
        /// <param name="type">Tipo a comprobar.</param>
        /// <returns><c>true</c> si es diccionario; en caso contrario, <c>false</c>.</returns>
        public static bool IsDictionary(this Type type)
        {
            return type.GetInterfaces()
                .Union(new[] { type })
                .Any(x => x == typeof(IDictionary) || (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)));
        }

        /// <summary>
        /// Determina si el tipo especificado es un diccionario y obtiene los tipos de clave y valor.
        /// </summary>
        /// <param name="type">Tipo a comprobar.</param>
        /// <param name="keyType">Tipo de la clave.</param>
        /// <param name="elementType">Tipo del valor.</param>
        /// <returns><c>true</c> si es diccionario; en caso contrario, <c>false</c>.</returns>
        public static bool IsDictionary(this Type type, out Type keyType, out Type elementType)
        {
            keyType = type;
            elementType = type;

            var collectionType = type
                .GetInterfaces()
                .Union(new[] { type })
                .FirstOrDefault(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IDictionary<,>)));

            if (collectionType is null)
                return false;

            var arguments = collectionType.GetGenericArguments();
            keyType = arguments.First();
            elementType = arguments.Skip(1).First();

            return true;
        }

        /// <summary>
        /// Intenta convertir un valor de un tipo a otro, manejando conversiones comunes y tipos especiales como Nullable, Enum y arrays de bytes.
        /// </summary>
        /// <param name="desiredType">Tipo de destino.</param>
        /// <param name="valueType">Tipo original del valor.</param>
        /// <param name="value">Valor a convertir.</param>
        /// <returns>Valor convertido o <c>null</c> si no es posible la conversión.</returns>
        public static object? CoerceValue(Type desiredType, Type valueType, object value)
        {
            // types match, just copy value
            if (desiredType == valueType)
                return value;

            bool isNullable = desiredType.IsGenericType && (desiredType.GetGenericTypeDefinition() == _nullableType);
            if (isNullable)
            {
                if (value is null)
                    return null;
                if (_stringType == valueType && Convert.ToString(value) == String.Empty)
                    return null;
            }

            desiredType = GetUnderlyingType(desiredType);

            if ((desiredType.IsPrimitive || typeof(decimal) == desiredType)
                && _stringType == valueType
                && string.IsNullOrEmpty((string)value))
                return 0;

            if (value is null)
                return null;

            // types don't match, try to convert
            if (typeof(Guid) == desiredType)
                return Guid.TryParse(value.ToString(), out Guid guid) ? guid : null;

            if (desiredType.IsEnum && _stringType == valueType)
                return Enum.Parse(desiredType, value.ToString() ?? string.Empty, true);

            bool isBinary = desiredType.IsArray && _byteArrayType == desiredType;

            if (isBinary && _stringType == valueType)
            {
                byte[] bytes = Convert.FromBase64String((string)value);
                return bytes;
            }

            isBinary = valueType.IsArray && _byteArrayType == valueType;

            if (isBinary && _stringType == desiredType)
            {
                byte[] bytes = (byte[])value;
                return Convert.ToBase64String(bytes);
            }

            try
            {
                if (_stringType == desiredType)
                    return value.ToString();

                return Convert.ChangeType(value, desiredType, Thread.CurrentThread.CurrentCulture);
            }
            catch
            {
#if !SILVERLIGHT
                TypeConverter converter = TypeDescriptor.GetConverter(desiredType);
                if (converter is not null && converter.CanConvertFrom(valueType))
                    return converter.ConvertFrom(value);
#endif
                throw;
            }
        }

        /// <summary>
        /// Determina si el método especificado es una sobreescritura de un método base.
        /// </summary>
        /// <param name="method">Información del método.</param>
        /// <returns><c>true</c> si el método sobreescribe un método base; en caso contrario, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="method"/> es <c>null</c>.</exception>
        public static bool IsOverriding(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.DeclaringType != method.GetBaseDefinition().DeclaringType;
        }

    }
}
