using Postulate.Mvc;
using Sample.Models;
using SampleWebApp.Queries;
using SampleWebApp.SelectListQueries;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class CustomerController : ControllerBase
    {
        public ActionResult Index(AllCustomers query)
        {
            FillSelectLists(query);

            query.OrgId = CurrentUser.OrganizationId;
            var list = query.Execute();
            return View(list);
        }

        protected override IEnumerable<SelectListQuery> SelectListQueries(object record = null)
        {
            return new SelectListQuery[]
            {
                new RegionSelect(),
                new CustomerTypeSelect()
            };
        }

        public ActionResult Create(Customer record)
        {
            FillSelectLists(record);

            return View(record);
        }

        public ActionResult Edit(int id)
        {
            var record = Db.Find<Customer>(id);

            FillSelectLists(record);

            return View(record);
        }

        public ActionResult Save(Customer record, string actionName)
        {
            record.OrganizationId = CurrentUser.OrganizationId;

            if (SaveRecord(record)) return RedirectToAction("Edit", new { id = record.Id });

            // if you made it here, it means something went wrong
            FillSelectLists(record);
            return RedirectToAction(actionName, record);
        }

        public ActionResult Delete(int id)
        {
            var record = Db.Find<Customer>(id);
            return View(record);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (DeleteRecord<Customer>(id)) return RedirectToAction("Index");

            return RedirectToAction("Delete", new { id = id });
        }

        public ActionResult Changes(int id)
        {
            var history = Db.QueryChangeHistory<Customer>(id);
            return PartialView(history);
        }
    }
}