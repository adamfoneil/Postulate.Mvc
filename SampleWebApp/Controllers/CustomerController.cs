using Postulate.Mvc;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using SampleWebApp.Queries;

namespace SampleWebApp.Controllers
{
    public class CustomerController : SqlServerDbController<DemoDb2, int>
    {        
        public ActionResult Index()
        {
            var list = new AllCustomers().Execute();
            return View(list);
        }

        public ActionResult Create(Customer record)
        {
            return View(record);
        }

        public ActionResult Edit(int id)
        {
            var record = _db.Find<Customer>(id);
            return View(record);
        }

        public ActionResult Save(Customer record, string actionName)
        {
            if (SaveRecord(record)) return RedirectToAction("Edit", new { id = record.Id });
            return RedirectToAction(actionName, record);
        }
    }
}