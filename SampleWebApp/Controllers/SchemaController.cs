using Microsoft.AspNet.Identity.Owin;
using Postulate.Mvc;
using Postulate.Orm.Merge;
using Postulate.Orm.SqlServer;
using Sample.Models;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class SchemaController : SampleWebApp.ControllerBase
    {
        private Engine<SqlServerSyntax> _merge = new Engine<SqlServerSyntax>(Startup.GetModelAssembly());        

        public ActionResult Index()
        {
            return View();
        }

        private ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().Get<ApplicationUserManager>(); }
        }

        public async Task<ActionResult> Preview()
        {
            using (var cn = Db.GetConnection())
            {
                cn.Open();
                var actions = await _merge.CompareAsync(cn);
                var script = _merge.GetScript(cn, actions);
                return View(script);
            }
        }

        [HttpPost]
        [ActionName("Preview")]
        public async Task<ActionResult> Execute()
        {            
            try
            {
                using (var cn = Db.GetConnection())
                {
                    cn.Open();
                    if (!CurrentUser.HasRole(cn, "Admins")) throw new Exception("You must be in Admins role to execute schema merges");
                    var actions = await _merge.CompareAsync(cn);
                    await _merge.ExecuteAsync(cn, actions);
                }

                TempData.Add("success", "Schema merge executed successfully!");
            }
            catch (Exception exc)
            {
                TempData.Add("error", exc.Message);
            }
            return RedirectToAction("Preview");
        }
    }
}