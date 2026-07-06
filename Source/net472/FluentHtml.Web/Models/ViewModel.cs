using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FluentHtml.Web.Models
{
    public class ViewModel
    {
        private List<ChildModel> _childs;

        [Display(Name = "Property A")]
        public string PropA { get; set; }

        [Display(Name = "Property B")]
        [Required]
        public int PropB { get; set; }
        public string PropC { get; set; }
        public Dictionary<int, string> PropCOptions { get; set; }
        public bool PropD { get; set; }
        public ChildModel Child { get; set; }

        public List<ChildModel> Childs
        {
            get
            {
                return this._childs ?? (this._childs = new List<ChildModel>());
            }
            set
            {
                if (this._childs != value)
                    this._childs = value;
            }
        }
    }

    public class ChildModel
    {
        [Display(Name = "Child Property A")]
        [StringLength(100)]
        public string ChildPropA { get; set; }
    }
}