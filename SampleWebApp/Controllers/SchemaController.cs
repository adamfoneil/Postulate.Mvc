using Postulate.Orm.Merge;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class SchemaController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Merge()
        {
            var sm = new SchemaMerge<DemoDb>();
            sm.Compare();
            return View(sm);
        }

        [HttpPost]
        [ActionName("Merge")]
        public ActionResult MergeExecute()
        {
            try
            {
                var sm = new SchemaMerge<DemoDb>();
                sm.Execute();
                TempData.Add("success", "Schema merge executed successfully!");
            }
            catch (Exception exc)
            {
                TempData.Add("error", exc.Message);
            }
            return RedirectToAction("Merge");
        }
    }
}