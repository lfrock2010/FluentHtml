using System;
using System.Collections.Generic;

namespace FluentHtml.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IDictionary{TKey, TValue}"/> to simplify common operations.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds a key/value pair to the Dictionary if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to perform the operation on.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
            where TKey : notnull
        {
            TValue? value;
            if (dictionary.TryGetValue(key, out value))
                return value;

            value = valueFactory(key);
            dictionary.Add(key, value);

            return value;
        }

        /// <summary>
        /// Attempts to add the specified key and value to the Dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to perform an action upon.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>
        ///   <c>true</c> if the key/value pair was added to the Dictionary successfully. If the key already exists, this method returns <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if key is a <c>null</c> reference </exception>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
             where TKey : notnull
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (dictionary.ContainsKey(key))
                return false;

            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Attempts to remove and return the value with the specified key from the Dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The Dictionary to perform an action upon.</param>
        /// <param name="key">The key of the element to remove and return.</param>
        /// <param name="value">When this method returns, value contains the object removed from the Dictionary or the default value if the operation failed.</param>
        /// <returns><c>true</c> if an object was removed successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if key is a <c>null</c> reference </exception>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue? value)
             where TKey : notnull
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!dictionary.TryGetValue(key, out value))
                return false;

            return dictionary.Remove(key);
        }

        /// <summary>
        /// Compares the existing value for the specified key with a specified value, and if they are equal, updates the key with a third value.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to perform an action upon.</param>
        /// <param name="key">The key whose value is compared with comparisonValue and possibly replaced.</param>
        /// <param name="newValue">The value that replaces the value of the element with key if the comparison results in equality.</param>
        /// <param name="comparisonValue">The value that is compared to the value of the element with key.</param>
        /// <returns><c>true</c> if the value with key was equal to comparisonValue and replaced with newValue; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if key is a <c>null</c> reference </exception>
        public static bool TryUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue, TValue comparisonValue)
             where TKey : notnull
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            TValue? value;
            if (!dictionary.TryGetValue(key, out value))
                return false;

            if (!Equals(value, comparisonValue))
                return false;

            dictionary[key] = newValue;
            return true;
        }

        /// <summary>
        /// Merges the key/value pairs from the specified collection into the current dictionary, with the option to replace existing values.
        /// </summary>
        /// <param name="instance">The target dictionary where elements will be merged.</param>
        /// <param name="from">Collection of key/value pairs to merge.</param>
        /// <param name="replaceExisting">If <c>true</c>, existing values will be replaced; if <c>false</c>, current values will be preserved.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="instance"/> or <paramref name="from"/> is <c>null</c>.</exception>
        public static void Merge(this IDictionary<string, object?> instance, IEnumerable<KeyValuePair<string, object?>> from, bool replaceExisting)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (@from is null)
                throw new ArgumentNullException(nameof(from));

            foreach (KeyValuePair<string, object?> pair in from)
            {
                if (String.IsNullOrEmpty(pair.Key) || (!replaceExisting && instance.ContainsKey(pair.Key)))
                    continue;

                if (pair.Value is not null)
                    instance[pair.Key] = pair.Value;
                else
                {
                    if (instance.ContainsKey(pair.Key))
                        instance.Remove(pair.Key);
                }
            }
        }

        /// <summary>
        /// Merges the key/value pairs from the specified collection into the current dictionary, replacing existing values.
        /// </summary>
        /// <param name="instance">The target dictionary where elements will be merged.</param>
        /// <param name="from">Collection of key/value pairs to merge.</param>
        public static void Merge(this IDictionary<string, object?> instance, IEnumerable<KeyValuePair<string, object?>> from)
        {
            Merge(instance, from, true);
        }

        /// <summary>
        /// Merges the key/value pairs from the specified dictionary into the current dictionary, replacing existing values.
        /// </summary>
        /// <param name="instance">The target dictionary where elements will be merged.</param>
        /// <param name="from">Dictionary of key/value pairs to merge.</param>
        public static void Merge(this IDictionary<string, object?> instance, IDictionary<string, object?> from)
        {
            Merge(instance, from, true);
        }

        /// <summary>
        /// Merges the public properties of the specified object into the current dictionary, with the option to replace existing values.
        /// </summary>
        /// <param name="instance">The target dictionary where elements will be merged.</param>
        /// <param name="values">Object whose public properties will be added as key/value pairs.</param>
        /// <param name="replaceExisting">If <c>true</c>, existing values will be replaced; if <c>false</c>, current values will be preserved.</param>
        public static void Merge(this IDictionary<string, object?> instance, object values, bool replaceExisting)
        {
            Merge(instance, values.ToDictionary(), replaceExisting);
        }

        /// <summary>
        /// Merges the public properties of the specified object into the current dictionary, replacing existing values.
        /// </summary>
        /// <param name="instance">The target dictionary where elements will be merged.</param>
        /// <param name="values">Object whose public properties will be added as key/value pairs.</param>
        public static void Merge(this IDictionary<string, object?> instance, object values)
        {
            Merge(instance, values, true);
        }
    }
}