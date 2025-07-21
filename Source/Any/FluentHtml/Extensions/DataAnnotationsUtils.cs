#if NETSTANDARD
#elif NETCOREAPP
#else
using System.Web.Mvc;
#endif

namespace System.ComponentModel.DataAnnotations
{
    internal static class DataAnnotationsUtils
    {
        public static bool HasValidMaximumValue(this RangeAttribute rangeAttribute)
        {
            if (rangeAttribute is null)
                throw new ArgumentNullException(nameof(rangeAttribute));

            return DataAnnotationsUtils.IsValidValue(rangeAttribute.Maximum);
        }

        public static bool HasValidMinimumValue(this RangeAttribute rangeAttribute)
        {
            if (rangeAttribute is null)
                throw new ArgumentNullException(nameof(rangeAttribute));

            return DataAnnotationsUtils.IsValidValue(rangeAttribute.Minimum);
        }

#if !NETSTANDARD && !NETCOREAPP
        public static bool HasValidValidationParameter(this ModelClientValidationRule modelClientValidationRule, string key)
        {
            if (modelClientValidationRule is null)
                throw new ArgumentNullException(nameof(modelClientValidationRule));

            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (!modelClientValidationRule.ValidationParameters.ContainsKey(key))
                return false;

            return DataAnnotationsUtils.IsValidValue(modelClientValidationRule.ValidationParameters[key]);
        }
#endif

        private static bool IsValidValue(object value)
        {
            if (value is null)
                return false;

            if (value is string str && (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str)))
                return false;

            return true;
        }
    }
}
