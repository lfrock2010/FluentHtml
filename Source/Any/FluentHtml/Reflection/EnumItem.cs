using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FluentHtml.Reflection
{
    /// <summary>
    /// Representa un valor de enumeración (Enum) de forma genérica.
    /// </summary>
    public class EnumItem : EnumItem<object>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EnumItem"/>.
        /// </summary>
        /// <param name="name">Nombre del valor de la enumeración.</param>
        /// <param name="description">Descripción asociada al valor de la enumeración.</param>
        /// <param name="underlyingValue">Valor subyacente del campo de la enumeración.</param>
        /// <param name="value">Valor original de la enumeración.</param>
        protected EnumItem(string name, string description, object underlyingValue, object value)
           : base(name, description, underlyingValue, value)
        {
        }
    }

    /// <summary>
    /// Representa un valor de enumeración (Enum) con metadatos adicionales como nombre, descripción y valor subyacente.
    /// </summary>
    /// <typeparam name="TEnum">Tipo de la enumeración.</typeparam>
    public class EnumItem<TEnum>
        : IEquatable<EnumItem<TEnum>>
    {
        private static readonly Lazy<IList<EnumItem<TEnum>>> _items = new Lazy<IList<EnumItem<TEnum>>>(() =>
            CreateList().ToList().AsReadOnly());

        /// <summary>
        /// Obtiene o establece el nombre del valor de la enumeración.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Obtiene o establece la descripción asociada al valor de la enumeración, tomada del <see cref="DescriptionAttribute"/>.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Obtiene o establece el valor subyacente del campo de la enumeración.
        /// </summary>
        public object UnderlyingValue { get; }

        /// <summary>
        /// Obtiene o establece el valor de la enumeración.
        /// </summary>
        public TEnum Value { get; }        

        /// <summary>
        /// Obtiene una lista de solo lectura de todos los valores de la enumeración <typeparamref name="TEnum"/> representados como <see cref="EnumItem{TEnum}"/>.
        /// </summary>
        public static IList<EnumItem<TEnum>> Items
        {
            get { return _items.Value; }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EnumItem{TEnum}"/>.
        /// </summary>
        /// <param name="name">Nombre del valor de la enumeración.</param>
        /// <param name="description">Descripción asociada al valor de la enumeración.</param>
        /// <param name="underlyingValue">Valor subyacente del campo de la enumeración.</param>
        /// <param name="value">Valor original de la enumeración.</param>
        protected EnumItem(string name, string description, object underlyingValue, TEnum value)
        {
            this.Name = name;
            this.Description = description;
            this.UnderlyingValue = underlyingValue;
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Devuelve una cadena que representa la instancia actual, mostrando la descripción si está disponible, o el nombre.
        /// </summary>
        /// <returns>Descripción o nombre del valor de la enumeración.</returns>
        public override string ToString()
        {
            return Description ?? Name;
        }

        /// <summary>
        /// Determina si el objeto especificado es igual a la instancia actual de <see cref="EnumItem{TEnum}"/>.
        /// </summary>
        /// <param name="other">Instancia a comparar.</param>
        /// <returns><c>true</c> si ambos objetos representan el mismo valor de enumeración; en caso contrario, <c>false</c>.</returns>
        public bool Equals(EnumItem<TEnum>? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (this.Value is null)
                return other.Value is null;

            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Determina si el objeto especificado es igual a la instancia actual.
        /// </summary>
        /// <param name="obj">Objeto a comparar.</param>
        /// <returns><c>true</c> si ambos objetos son iguales; en caso contrario, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((EnumItem<TEnum>)obj);
        }

        /// <summary>
        /// Devuelve un código hash para la instancia actual.
        /// </summary>
        /// <returns>Código hash basado en el valor de la enumeración.</returns>
        public override int GetHashCode()
        {
            if (this.Value is null)
                return 0;

            return Value.GetHashCode();
        }

        /// <summary>
        /// Compara dos instancias de <see cref="EnumItem{TEnum}"/> para determinar si son iguales.
        /// </summary>
        public static bool operator ==(EnumItem<TEnum>? left, EnumItem<TEnum>? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compara dos instancias de <see cref="EnumItem{TEnum}"/> para determinar si son diferentes.
        /// </summary>
        public static bool operator !=(EnumItem<TEnum>? left, EnumItem<TEnum>? right)
        {
            return !Equals(left, right);
        }        

        /// <summary>
        /// Crea una instancia de <see cref="EnumItem{TEnum}"/> a partir de un valor de enumeración especificado.
        /// </summary>        
        /// <param name="enumValue">Valor de la enumeración.</param>
        /// <returns>Instancia de <see cref="EnumItem{TEnum}"/> correspondiente al valor, o <c>null</c> si no se encuentra.</returns>
        public static EnumItem<TEnum>? Create(TEnum enumValue)
        {
            if (enumValue is null)
                return null;

            return Items.FirstOrDefault(i => enumValue.Equals(i.Value));
        }

        /// <summary>
        /// Crea una lista de <see cref="EnumItem{TEnum}"/> para todos los valores definidos en la enumeración <typeparamref name="TEnum"/>.
        /// </summary>
        /// <returns>Enumeración de <see cref="EnumItem{TEnum}"/>.</returns>
        public static IEnumerable<EnumItem<TEnum>> CreateList()
        {
            var enumType = typeof(TEnum);
            return CreateList(enumType);
        }

        /// <summary>
        /// Crea una lista de <see cref="EnumItem{TEnum}"/> para todos los valores definidos en el tipo de enumeración especificado.
        /// </summary>
        /// <param name="enumType">Tipo de la enumeración.</param>
        /// <returns>Enumeración de <see cref="EnumItem{TEnum}"/>.</returns>
        public static IEnumerable<EnumItem<TEnum>> CreateList(Type enumType)
        {
            return from field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                   let attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                       .OfType<DescriptionAttribute>()
                       .FirstOrDefault()
                   let description = attribute is null ? field.Name : attribute.Description
                   let underlyingValue = field.GetRawConstantValue()
                   let original = Enum.ToObject(enumType, underlyingValue)
                   select new EnumItem<TEnum>
                   (
                       name: field.Name,
                       description: description,
                       underlyingValue: underlyingValue,
                       value: (TEnum)original
                   );
        }
    }

}