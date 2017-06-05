using Postulate.Mvc;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp.Controllers
{
    public class CustomerController : SqlServerDbController<DemoDb, int>
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
    }
}