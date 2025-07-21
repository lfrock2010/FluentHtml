using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System.Web.Mvc
{
    internal static class ModelMetadataUtils
    {
        private const string COLLECTION_EXPRESSION_PATTERN = @"\[\d+\](.|$)";
        private const string PROPERTY_COLLECTION_PATTERN = @"\[(\d+)\]$";

        public static ModelMetadata FromLambdaExpression<TParameter, TValue>(Expression<Func<TParameter, TValue>> expression, ViewDataDictionary<TParameter> viewData)
        {
            return ModelMetadata.FromLambdaExpression<TParameter, TValue>(expression, viewData);
        }

        private static Func<object> GetElementAtAccesor(object containerValue, Type modelType, int index)
        {
            return () =>
            {
                object modelMetadataValue = null;

                try
                {
                    if (!object.ReferenceEquals(containerValue, null))
                    {
                        if (containerValue is Array)
                        {
                            Array arrayContainerValue = (Array)containerValue;
                            modelMetadataValue = arrayContainerValue.GetValue(index);
                        }
                        else if (containerValue is IList)
                        {
                            IList listContainerValue = (IList)containerValue;
                            modelMetadataValue = listContainerValue[index];
                        }
                        else if (typeof(IEnumerable<>).MakeGenericType(modelType).IsAssignableFrom(containerValue.GetType()))
                        {
                            modelMetadataValue = Enumerable.ElementAt((dynamic)containerValue, index);
                        }
                        else
                        {
                            IEnumerable enumerableContainerValue = (IEnumerable)containerValue;
                            IEnumerator enumerator = enumerableContainerValue.GetEnumerator();
                            int enumeratorIndex = 0;
                            while (enumerator.MoveNext())
                            {
                                if (enumeratorIndex == index)
                                    modelMetadataValue = enumerator.Current;
                            }
                        }
                    }
                }
                catch
                {
                }

                return modelMetadataValue;
            };
        }

        private static Func<object> GetPropertyValueAccesor(Type containerType, object containerValue, string propertyName)
        {
            return () =>
            {
                object modelMetadataValue = null;

                try
                {
                    if (!object.ReferenceEquals(containerValue, null))
                    {
                        PropertyInfo propertyInfo = containerType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfo != null)
                            modelMetadataValue = propertyInfo.GetValue(containerValue, null);
                    }
                }
                catch
                {
                }

                return modelMetadataValue;
            };
        }

        private static bool TryGetElementTypeFromEnumerable(Type enumerableType, ref Type elementType)
        {
            bool isEnumerable = false;
            elementType = null;
            Type typeToValidate = enumerableType;
            while (true)
            {
                if (typeToValidate.IsArray)
                {
                    elementType = typeToValidate.GetElementType();
                    isEnumerable = true;
                    break;
                }
                else if (typeToValidate.IsGenericType)
                {
                    elementType = typeToValidate.GetGenericArguments()[0];
                    Type enumerableConcreteType = typeof(IEnumerable<>).MakeGenericType(elementType);
                    isEnumerable = enumerableConcreteType.IsAssignableFrom(typeToValidate);
                    break;
                }

                typeToValidate = typeToValidate.BaseType;
                if (typeToValidate == null || typeToValidate == typeof(object))
                    break;
            }

            return isEnumerable;
        }

        public static void FromStringExpression(string expression, ViewDataDictionary viewData, out ModelMetadata modelMetaData, out string propertyName)
        {
            propertyName = expression;
            modelMetaData = null;

            if (Regex.IsMatch(expression, ModelMetadataUtils.COLLECTION_EXPRESSION_PATTERN, RegexOptions.IgnoreCase))
            {
                try
                {
                    Regex regex = new Regex(ModelMetadataUtils.PROPERTY_COLLECTION_PATTERN, RegexOptions.IgnoreCase);
                    ModelMetadataProvider metadataProvider = ModelMetadataProviders.Current;
                    string[] expressions = expression.Split('.');

                    for (int i = 0; i < expressions.Length; i++)
                    {
                        int? index = null;
                        propertyName = expressions[i];
                        Match m = regex.Match(propertyName);
                        if (m.Success)
                        {
                            index = Convert.ToInt32(m.Groups[1].Value);
                            propertyName = regex.Replace(propertyName, string.Empty);
                        }

                        if (i > 0)
                        {
                            Type containerType = modelMetaData.ModelType;
                            object containerValue = modelMetaData.Model;
                            Func<object> modelAccessor = ModelMetadataUtils.GetPropertyValueAccesor(containerType, containerValue, propertyName);
                            modelMetaData = metadataProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName);
                        }
                        else
                            modelMetaData = ModelMetadata.FromStringExpression(propertyName, viewData);

                        if (modelMetaData == null)
                            break;

                        if (index.HasValue)
                        {
                            Type modelType = null;
                            bool isEnumerable = ModelMetadataUtils.TryGetElementTypeFromEnumerable(modelMetaData.ModelType, ref modelType);
                            if (isEnumerable)
                            {
                                object containerValue = modelMetaData.Model;
                                Func<object> modelAccessor = ModelMetadataUtils.GetElementAtAccesor(containerValue, modelType, index.Value);
                                modelMetaData = metadataProvider.GetMetadataForType(modelAccessor, modelType);
                                if (modelMetaData == null)
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    modelMetaData = ModelMetadata.FromStringExpression(expression, viewData);
                }
            }
            else
                modelMetaData = ModelMetadata.FromStringExpression(expression, viewData);
        }
    }
}
