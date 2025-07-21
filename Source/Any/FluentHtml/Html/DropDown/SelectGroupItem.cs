using System;
using System.Collections.Generic;

#if !NETCOREAPP && !NETSTANDARD
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

namespace FluentHtml.Html.DropDown
{
    /// <summary>
    /// Representa un grupo de elementos para un control <c>&lt;select&gt;</c> (optgroup),
    /// permitiendo agrupar varias opciones bajo una misma etiqueta.
    /// </summary>
    public class SelectGroupItem
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SelectGroupItem"/>.
        /// </summary>
        public SelectGroupItem()
        {
            Items = new List<SelectListItem>();
        }

        /// <summary>
        /// Etiqueta que se mostrará como título del grupo en el control <c>&lt;select&gt;</c>.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Lista de elementos <see cref="SelectListItem"/> que pertenecen a este grupo.
        /// </summary>
        public List<SelectListItem> Items { get; set; }
    }
}