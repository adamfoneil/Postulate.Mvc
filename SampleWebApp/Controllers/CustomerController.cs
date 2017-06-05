using Postulate.Mvc;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace SampleWebApp.Controllers
{
    public class CustomerController : SqlServerDbController<DemoDb2, int>
    {        
        public ActionResult Index()
        {
            using (var cn = _db.GetConnection())
            {
                cn.Open();
                var list = cn.Query<Customer>("SELECT * FROM [Customer] ORDER BY [LastName]");
                return View(list);
            }
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