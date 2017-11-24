using Postulate.Mvc;
using Postulate.Orm.Merge;
using Postulate.Orm.SqlServer;
using Sample.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class SchemaController : ControllerBase<DemoDb, int>
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Merge()
        {
            using (var cn = Db.GetConnection())
            {
                cn.Open();
                var engine = new Engine<SqlServerSyntax>(Assembly.GetExecutingAssembly(), new Progress<MergeProgress>(ShowProgress));
                var actions = await engine.CompareAsync(cn);
                string script = engine.GetScript(cn, actions).ToString();
                return View(script);
            }
        }

        private void ShowProgress(MergeProgress obj)
        {
            // not much to do in web app
        }

        [HttpPost]
        [ActionName("Merge")]
        public async Task<ActionResult> MergeExecute()
        {
            try
            {
                using (var cn = Db.GetConnection())
                {
                    cn.Open();
                    var engine = new Engine<SqlServerSyntax>(Assembly.GetExecutingAssembly(), new Progress<MergeProgress>(ShowProgress));
                    var actions = await engine.CompareAsync(cn);
                    await engine.ExecuteAsync(cn, actions);
                }                
                
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