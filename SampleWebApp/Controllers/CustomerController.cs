using Postulate.Mvc;
using Sample.Models;
using System.Web.Mvc;
using SampleWebApp.Queries;
using SampleWebApp.SelectListQueries;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using AdamOneilSoftware;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class CustomerController : BaseController<DemoDb2, int, UserProfile>
    {        
        public ActionResult Index(AllCustomers query)
        {
            FillSelectLists(query);

            query.OrgId = CurrentUser.OrganizationId;
            var list = query.Execute();
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

        public ActionResult Generate(int count = 100)
        {
            int[] orgIds = null;
            CustomerType[] customerTypes = null;
            int[] regionIds = null;

            using (var cn = Db.GetConnection())
            {
                cn.Open();
                orgIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Organization]").ToArray();
                customerTypes = cn.Query<CustomerType>("SELECT * FROM [dbo].[CustomerType]").ToArray();
                regionIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Region]").ToArray();
            }
            
            var tdg = new TestDataGenerator();
            tdg.Generate<Customer>(count, (c) =>
            {
                c.OrganizationId = tdg.Random(orgIds);
                c.LastName = tdg.Random(Source.LastName);
                c.FirstName = tdg.Random(Source.FirstName);
                c.Address = tdg.Random(Source.Address);
                c.TypeId = tdg.Random<CustomerType>(customerTypes, t => t.OrganizationId == c.OrganizationId).Id;
                c.RegionId = tdg.Random(regionIds);
                c.CreatedBy = "random";
            }, (records) =>
            {
                Db.SaveMultiple(records);
            });

            return View(count);
        }
    }
}