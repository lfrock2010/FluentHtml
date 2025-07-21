using System;
using System.Collections.Generic;
using System.Linq;
#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
using System.Web.Routing;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using RequestContext = System.Web.Routing.RequestContext;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using RequestContext = Microsoft.AspNetCore.Mvc.Rendering.FluentRequestContext;
#endif


namespace FluentHtml.Fluent
{
    /// <summary>
    /// Clase base abstracta para vistas seguras con control de acceso por roles
    /// </summary>
    public abstract class SecureViewBase : ViewComponentBase
    {
        /// <summary>
        /// Constructor que inicializa los roles y la clase CSS por defecto para acceso denegado
        /// <param name="fluentHelper">The <see cref="FluentHelper"/> instance to associate with this component.</param> 
        /// </summary>
        protected SecureViewBase(FluentHelper fluentHelper)
            : base(fluentHelper)
        {
            ReadRoles = new HashSet<string>();
            WriteRoles = new HashSet<string>();
            DeniedClass = "access-denied";
        }

        /// <summary>
        /// Conjunto de roles permitidos para lectura
        /// </summary>
        public ISet<string> ReadRoles { get; private set; }

        /// <summary>
        /// Conjunto de roles permitidos para escritura
        /// </summary>
        public ISet<string> WriteRoles { get; private set; }

        /// <summary>
        /// Clase CSS que se aplica cuando el acceso es denegado
        /// </summary>
        public string DeniedClass { get; set; }

        /// <summary>
        /// Clase CSS que se aplica cuando el acceso es concedido (opcional)
        /// </summary>
        public string? GrantedClass { get; set; }

        /// <summary>
        /// Indica si la vista está protegida por roles (true si no hay roles definidos)
        /// </summary>
        public bool IsSecured()
        {
            return ReadRoles.Count == 0 || WriteRoles.Count == 0;
        }

        /// <summary>
        /// Verifica si el usuario actual tiene permiso de lectura
        /// </summary>
        public bool CanRead()
        {
            if (ReadRoles.Count == 0
                || RequestContext is null
                || RequestContext.HttpContext is null
                || RequestContext.HttpContext.User is null)
            {
                return true;
            }

            var principal = RequestContext.HttpContext.User;
            return ReadRoles.Any(principal.IsInRole);

        }

        /// <summary>
        /// Verifica si el usuario actual tiene permiso de escritura
        /// </summary>
        public bool CanWrite()
        {
            if (WriteRoles.Count == 0
                || RequestContext is null
                || RequestContext.HttpContext is null
                || RequestContext.HttpContext.User is null)
            {
                return true;
            }

            var principal = RequestContext.HttpContext.User;
            return WriteRoles.Any(principal.IsInRole);
        }
    }
}