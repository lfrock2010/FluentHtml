using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using FluentHtml;


#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
#elif NETCOREAPP
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

#if NETSTANDARD
namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
#elif NETCOREAPP
namespace Microsoft.AspNetCore.Mvc.ViewFeatures
#else
namespace System.Web.Mvc
#endif
{
    internal static class ModelExplorerUtils
    {
        private const string COLLECTION_EXPRESSION_PATTERN = @"\[\d+\](.|$)";
        private const string PROPERTY_COLLECTION_PATTERN = @"\[(\d+)\]$";

#if NETSTANDARD
        //public static ModelExplorer FromLambdaExpression<TParameter, TValue>(Expression<Func<TParameter, TValue>> expression, ViewDataDictionary<TParameter> viewData, IModelMetadataProvider metadataProvider)
        //{
        //    return ExpressionMetadataProvider.FromLambdaExpression<TParameter, TValue>(expression, viewData, metadataProvider);
        //}

        public static ModelExplorer FromLambdaExpression<TParameter, TValue>(Expression<Func<TParameter, TValue>> expression, IHtmlHelper<TParameter> helper)
        {
            return ExpressionMetadataProvider.FromLambdaExpression<TParameter, TValue>(expression, helper.ViewData, helper.MetadataProvider);
        }
#elif NETCOREAPP
        public static ModelExplorer FromLambdaExpression<TParameter, TValue>(Expression<Func<TParameter, TValue>> expression, IHtmlHelper<TParameter> helper)
        {
            var expresionProvider = helper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            ModelExpression modelExpression = expresionProvider.CreateModelExpression(helper.ViewData, expression);
            return modelExpression.ModelExplorer;
        }
#else
        public static ModelMetadata FromLambdaExpression<TParameter, TValue>(Expression<Func<TParameter, TValue>> expression, ViewDataDictionary<TParameter> viewData)
        {
            return ModelMetadata.FromLambdaExpression<TParameter, TValue>(expression, viewData);
        }
#endif

        private static bool TryGetElementTypeFromEnumerable(Type enumerableType, out Type? elementType)
        {
            bool isEnumerable = false;
            elementType = null;
            Type? typeToValidate = enumerableType;
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
                if (typeToValidate is null || typeToValidate == typeof(object))
                    break;
            }

            return isEnumerable;
        }

#if NETSTANDARD
        public static void FromStringExpression(string expression, IHtmlHelper helper, out ModelExplorer? modelExplorer, out string propertyName)
        {
            propertyName = expression;
            modelExplorer = null;
            object? modelValue = null;

            if (Regex.IsMatch(expression, ModelExplorerUtils.COLLECTION_EXPRESSION_PATTERN, RegexOptions.IgnoreCase))
            {
                try
                {
                    Regex regex = new Regex(ModelExplorerUtils.PROPERTY_COLLECTION_PATTERN, RegexOptions.IgnoreCase);                    
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
                            if (modelExplorer is null)
                                modelExplorer = ExpressionMetadataProvider.FromStringExpression(propertyName, helper.ViewData, helper.MetadataProvider);

                            Type containerType = modelExplorer.Metadata.ModelType;                            
                            ModelMetadata modelMetaData = helper.MetadataProvider.GetMetadataForProperty(containerType, propertyName);                            

                            //TODO: CHECAR, si modelMetaData.PropertyGetter PUEDE RECIBIR null, de ser asi modelValue solo se necesitaria aqui
                            //object modelValue = modelMetaData.PropertyGetter is not null ? modelMetaData.PropertyGetter(null) : null;
                            modelValue = modelMetaData.PropertyGetter is not null ? modelMetaData.PropertyGetter(modelValue ?? new object()) : null;
                            modelExplorer = new ModelExplorer(helper.MetadataProvider, modelMetaData, modelValue);
                        }
                        else                            
                            modelExplorer = ExpressionMetadataProvider.FromStringExpression(propertyName, helper.ViewData, helper.MetadataProvider);

                        if (modelExplorer is null)
                            break;

                        if (index.HasValue)
                        {                            
                            bool isEnumerable = ModelExplorerUtils.TryGetElementTypeFromEnumerable(modelExplorer.ModelType, out Type? modelType);
                            if (isEnumerable)
                            {
                                ModelMetadata modelMetaData = helper.MetadataProvider.GetMetadataForType(modelType!);
                                if (modelMetaData is null)
                                    break;

                                modelExplorer = new ModelExplorer(helper.MetadataProvider, modelMetaData, null);
                            }
                        }
                    }
                }
                catch
                {
                    modelExplorer = ExpressionMetadataProvider.FromStringExpression(expression, helper.ViewData, helper.MetadataProvider);                    
                }
            }
            else
                modelExplorer = ExpressionMetadataProvider.FromStringExpression(expression, helper.ViewData, helper.MetadataProvider);                
        }
#elif NETCOREAPP
        public static void FromStringExpression(string expression, IHtmlHelper helper, out ModelExplorer? modelExplorer, out string propertyName)
        {
            propertyName = expression;
            modelExplorer = null;
            object? modelValue = null;

            var expresionProvider = helper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            if (expresionProvider is null)
                throw new Exception(string.Format(Resources.ServiceNotAdded, typeof(ModelExpressionProvider)));

            if (Regex.IsMatch(expression, ModelExplorerUtils.COLLECTION_EXPRESSION_PATTERN, RegexOptions.IgnoreCase))
            {
                try
                {
                    Regex regex = new Regex(ModelExplorerUtils.PROPERTY_COLLECTION_PATTERN, RegexOptions.IgnoreCase);
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
                            if (modelExplorer is null)
                            {
                                ModelExpression modelExpression = expresionProvider.CreateModelExpression((dynamic)helper.ViewData, propertyName);
                                modelValue = modelExpression.Model;
                                modelExplorer = modelExpression.ModelExplorer;
                            }

                            Type containerType = modelExplorer.Metadata.ModelType;
                            ModelMetadata modelMetaData = helper.MetadataProvider.GetMetadataForProperty(containerType, propertyName);

                            //TODO: CHECAR, si modelMetaData.PropertyGetter PUEDE RECIBIR null, de ser asi modelValue solo se necesitaria aqui
                            //object modelValue = modelMetaData.PropertyGetter is not null ? modelMetaData.PropertyGetter(null) : null;
                            modelValue = modelMetaData.PropertyGetter is not null ? modelMetaData.PropertyGetter(modelValue ?? new object()) : null;
                            modelExplorer = new ModelExplorer(helper.MetadataProvider, modelMetaData, modelValue);
                        }
                        else
                        {
                            ModelExpression modelExpression = expresionProvider.CreateModelExpression((dynamic)helper.ViewData, propertyName);
                            modelExplorer = modelExpression.ModelExplorer;
                        }

                        if (modelExplorer is null)
                            break;

                        if (index.HasValue)
                        {                            
                            bool isEnumerable = ModelExplorerUtils.TryGetElementTypeFromEnumerable(modelExplorer.ModelType, out Type? modelType);
                            if (isEnumerable)
                            {
                                ModelMetadata modelMetaData = helper.MetadataProvider.GetMetadataForType(modelType!);
                                if (modelMetaData is null)
                                    break;

                                modelExplorer = new ModelExplorer(helper.MetadataProvider, modelMetaData, null);
                            }
                        }
                    }
                }
                catch
                {
                    //modelExplorer = ExpressionMetadataProvider.FromStringExpression(expression, helper.ViewData, helper.MetadataProvider);
                    ModelExpression modelExpression = expresionProvider.CreateModelExpression((dynamic)helper.ViewData, expression);
                    modelExplorer = modelExpression.ModelExplorer;
                }
            }
            else
            {                
                ModelExpression modelExpression = expresionProvider.CreateModelExpression((dynamic)helper.ViewData, expression);
                modelExplorer = modelExpression.ModelExplorer;
            }
        }
#else
        private static Func<object?> GetElementAtAccesor(object containerValue, Type modelType, int index)
        {
            return () =>
            {
                object? modelMetadataValue = null;

                try
                {
                    if (containerValue is not null)
                    {
                        if (containerValue is Array arrayContainerValue)
                        {                            
                            modelMetadataValue = arrayContainerValue.GetValue(index);
                        }
                        else if (containerValue is IList listContainerValue)
                        {                            
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

        private static Func<object?> GetPropertyValueAccesor(Type containerType, object containerValue, string propertyName)
        {
            return () =>
            {
                object? modelMetadataValue = null;

                try
                {
                    if (containerValue is not null)
                    {
                        PropertyInfo propertyInfo = containerType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (propertyInfo is not null)
                            modelMetadataValue = propertyInfo.GetValue(containerValue, null);
                    }
                }
                catch
                {
                }

                return modelMetadataValue;
            };
        }

        public static void FromStringExpression(string expression, ViewDataDictionary viewData, out ModelMetadata? modelMetaData, out string propertyName)
        {
            propertyName = expression;
            modelMetaData = null;

            if (Regex.IsMatch(expression, ModelExplorerUtils.COLLECTION_EXPRESSION_PATTERN, RegexOptions.IgnoreCase))
            {
                try
                {
                    Regex regex = new Regex(ModelExplorerUtils.PROPERTY_COLLECTION_PATTERN, RegexOptions.IgnoreCase);
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
                            if (modelMetaData is null)
                                modelMetaData = ModelMetadata.FromStringExpression(propertyName, viewData);

                            Type? containerType = modelMetaData.ModelType;
                            object containerValue = modelMetaData.Model;
                            Func<object?> modelAccessor = ModelExplorerUtils.GetPropertyValueAccesor(containerType, containerValue, propertyName);
                            modelMetaData = metadataProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName);
                        }
                        else
                            modelMetaData = ModelMetadata.FromStringExpression(propertyName, viewData);

                        if (modelMetaData is null)
                            break;

                        if (index.HasValue)
                        {                            
                            bool isEnumerable = ModelExplorerUtils.TryGetElementTypeFromEnumerable(modelMetaData.ModelType, out Type? modelType);
                            if (isEnumerable)
                            {
                                object containerValue = modelMetaData.Model;
                                Func<object?> modelAccessor = ModelExplorerUtils.GetElementAtAccesor(containerValue, modelType!, index.Value);
                                modelMetaData = metadataProvider.GetMetadataForType(modelAccessor, modelType!);
                                if (modelMetaData is null)
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
#endif
    }
}
