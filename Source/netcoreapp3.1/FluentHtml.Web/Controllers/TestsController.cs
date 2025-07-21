using FluentHtml.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FluentHtml.Web.Controllers
{
    public class TestsController : Controller
    {
        public TestsController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Forms()
        {
            return this.View();
        }

        public IActionResult Links()
        {
            return this.View();
        }

        public IActionResult MvcForms()
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