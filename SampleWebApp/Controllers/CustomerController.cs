﻿using Postulate.Mvc;
using Sample.Models;
using System.Web.Mvc;
using SampleWebApp.Queries;
using SampleWebApp.SelectListQueries;

namespace SampleWebApp.Controllers
{
    public class CustomerController : BaseController<DemoDb2, int, UserProfile>
    {        
        public ActionResult Index()
        {
            var list = new AllCustomers().Execute();
            return View(list);
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

        private void FillSelectLists(Customer record)
        {
            FillSelectListsInner(record, new { orgId = CurrentUser.OrganizationId }, new RegionSelect(), new CustomerTypeSelect());
        }

        public ActionResult Save(Customer record, string actionName)
        {
            record.OrganizationId = CurrentUser.OrganizationId;
            if (SaveRecord(record)) return RedirectToAction("Edit", new { id = record.Id });
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
    }
}