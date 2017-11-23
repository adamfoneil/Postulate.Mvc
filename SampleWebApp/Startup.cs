using AdamOneilSoftware;
using Dapper;
using Microsoft.Owin;
using Owin;
using Postulate.Orm.Abstract;
using Postulate.Orm.Extensions;
using Postulate.Orm.Merge;
using Postulate.Orm.SqlServer;
using Sample.Models;
using System;
using System.Data;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(SampleWebApp.Startup))]
namespace SampleWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var db = new DemoDb() { UserName = "system" };
            db.CreateIfNotExists((cn) =>
            {
                new Engine<SqlServerSyntax>(new Type[]
                {
                    typeof(Customer), typeof(CustomerType), typeof(Organization), typeof(Region), typeof(UserProfile)
                }).ExecuteAsync(cn).Wait();                

                new RegionSeedData().Generate(cn, db);

                GenerateRandomData(cn, db);
            });
        }

        private void GenerateRandomData(IDbConnection cn, SqlDb<int> db)
        {
            var tdg = new TestDataGenerator();

            int[] orgIds = null;
            CustomerType[] customerTypes = null;
            int[] regionIds = null;
            
            tdg.GenerateUpTo<Organization>(cn, 10,
                connection => connection.QuerySingle<int?>("SELECT COUNT(1) FROM [dbo].[Organization]") ?? 0,
                o =>
                {
                    o.Name = tdg.Random(Source.CompanyName);
                    o.CreatedBy = "random";
                }, (records) =>
                {
                    db.SaveMultiple(records);
                });
            

            tdg.GenerateUniqueUpTo<CustomerType>(cn, 25,
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
                    db.Save(record);
                });

            orgIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Organization] [org] WHERE EXISTS(SELECT 1 FROM [dbo].[CustomerType] WHERE [OrganizationId]=[org].[Id])").ToArray();
            customerTypes = cn.Query<CustomerType>("SELECT * FROM [dbo].[CustomerType]").ToArray();
            regionIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Region]").ToArray();            

            tdg.Generate<Customer>(100, (c) =>
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
                db.SaveMultiple(records);
            });
        }
    }
}
