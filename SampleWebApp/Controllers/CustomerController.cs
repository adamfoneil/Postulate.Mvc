using Postulate.Mvc;
using Sample.Models;
using System.Web.Mvc;
using SampleWebApp.Queries;
using SampleWebApp.SelectListQueries;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using AdamOneilSoftware;
using System;
using Postulate.Orm.Extensions;

namespace SampleWebApp.Controllers
{
    [Authorize]
    public class CustomerController : BaseProfileController<DemoDb, int, UserProfile>
    {        
        public ActionResult Index(AllCustomers query)
        {
            FillSelectLists(query);

            query.OrgId = CurrentUser.OrganizationId;
            var list = query.Execute();
            return View(list);
        }

        protected override string ProfileUpdateUrl => "/Manage/Index?ManageMessageId=ProfileMissing";

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

        public ActionResult Generate(int count = 100)
        {
            var tdg = new TestDataGenerator();            

            int[] orgIds = null;
            CustomerType[] customerTypes = null;
            int[] regionIds = null;
            
            using (var cn = Db.GetConnection())
            {
                cn.Open();

                tdg.GenerateUpTo<Organization>(cn, 5,
                    connection => connection.QuerySingle<int?>("SELECT COUNT(1) FROM [dbo].[Organization]") ?? 0,
                    o =>
                    {
                        o.Name = tdg.Random(Source.CompanyName);
                        o.CreatedBy = "random";
                    }, (records) =>
                    {
                        Db.SaveMultiple(records);                        
                    });

                orgIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Organization]").ToArray();

                tdg.GenerateUniqueUpTo<CustomerType>(cn, 12,
                    connection => connection.QuerySingle<int?>("SELECT COUNT(1) FROM [dbo].[CustomerType]") ?? 0,
                    ct =>
                    {
                        ct.OrganizationId = tdg.Random(orgIds);
                        ct.Name = tdg.Random(Source.WidgetName);
                        ct.CreatedBy = "random";
                    }, (connection, ct) =>
                    {
                        return connection.Exists("[dbo].[CustomerType] WHERE [Name]=@name", new { name = ct.Name });
                    }, (record) =>
                    {
                        Db.Save(record);
                    });
                
                customerTypes = cn.Query<CustomerType>("SELECT * FROM [dbo].[CustomerType]").ToArray();
                regionIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Region]").ToArray();
            }
                        
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