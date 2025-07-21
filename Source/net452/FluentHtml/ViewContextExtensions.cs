using System.Reflection;

namespace System.Web.Mvc
{
    public static class ViewContextExtensions
    {
        public static FormContext GetFormContextForClientValidation(this ViewContext viewContext)
        {
            if (viewContext == null)
                return null;

            MethodInfo methodInfo = typeof(ViewContext).GetMethod("GetFormContextForClientValidation", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
                return (FormContext)methodInfo.Invoke(viewContext, new object[] { });

            if (!viewContext.ClientValidationEnabled)
                return null;

            return viewContext.FormContext;
        }
    }
}
