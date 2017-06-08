using Postulate.Mvc;
using Sample.Models;
using System.Web.Mvc;
using SampleWebApp.Queries;
using SampleWebApp.SelectListQueries;
using System;
using System.Collections.Generic;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class CustomerController : BaseController<DemoDb2, int, UserProfile>
    {        
        public ActionResult Index()
        {
            var list = new AllCustomers().Execute();
            return View(list);
        }

        protected override IEnumerable<SelectListQuery> SelectListQueries()
        {
            return new SelectListQuery[]
            {
                new RegionSelect(),
                new CustomerTypeSelect()
            };
        }

        protected override object SelectListParameters()
        {
            return new { orgId = CurrentUser.OrganizationId };
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