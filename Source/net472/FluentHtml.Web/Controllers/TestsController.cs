using FluentHtml.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FluentHtml.Web.Controllers
{
    public class TestsController : Controller
    {
        // GET: Tests
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Forms()
        {
            return this.View();
        }

        public ActionResult Links()
        {
            return this.View();
        }

        public ActionResult MvcForms()
        {
            ViewModel model = new ViewModel
            {
                PropA = "A",
                PropB = 2,
                PropC = "Two",
                PropCOptions = new Dictionary<int, string>()
                {
                    { 1, "One"},
                    { 2, "Two"},
                    { 3, "Three"}
                },
                PropD = true,
                Child = new ChildModel()
                {
                    ChildPropA = "Child A"
                }
            };

            model.Childs.Add(new ChildModel()
            {
                ChildPropA = "Sub Child A1"
            });

            model.Childs.Add(new ChildModel()
            {
                ChildPropA = "Sub Child A2"
            });

            ModelState.AddModelError(string.Empty, "General error message.");
            ModelState.AddModelError("PropB", "Property B error message.");
            
            return this.View(model);
        }
    }
}