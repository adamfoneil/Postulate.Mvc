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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

[assembly: OwinStartupAttribute(typeof(SampleWebApp.Startup))]
namespace SampleWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var a in assemblies)
            {
                Debug.WriteLine(a.Name);
            }

            var db = new DemoDb() { UserName = "startup" };
            db.CreateIfNotExists((cn, created) =>
            {
                if (created)
                {
                    CreateBaseTables(cn);
                    new RegionSeedData().Generate(cn, db);
                    CreateRandomData(cn, db);
                }
            });
        }

        private static void CreateBaseTables(IDbConnection cn)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Single(a => a.Name.Equals("SampleModels"));
            var types = Engine<SqlServerSyntax>.GetModelTypes(Assembly.Load(assemblyName));
            new Engine<SqlServerSyntax>(types).ExecuteAsync(cn).Wait();
        }

        private void CreateRandomData(IDbConnection cn, SqlDb<int> db)
        {
            var tdg = new TestDataGenerator();            
            
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

            int[] allOrgIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Organization] [org]").ToArray();

            tdg.GenerateUniqueUpTo<CustomerType>(cn, 25,
                connection => connection.QuerySingle<int?>("SELECT COUNT(1) FROM [dbo].[CustomerType]") ?? 0,
                ct =>
                {
                    ct.OrganizationId = tdg.Random(allOrgIds);
                    ct.Name = tdg.Random(Source.WidgetName);
                    ct.CreatedBy = "random";
                }, (connection, ct) =>
                {
                    return connection.Exists("[dbo].[CustomerType] WHERE [Name]=@name", new { name = ct.Name });
                }, (record) =>
                {
                    db.Save(record);
                });

            // not every org will have CustomerTypes generated, and when generating customers, we need to pick only those orgs that have at least one CustomerType
            int[] customerOrgIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Organization] [org] WHERE EXISTS(SELECT 1 FROM [dbo].[CustomerType] WHERE [OrganizationId]=[org].[Id])").ToArray();
            CustomerType[] customerTypes = cn.Query<CustomerType>("SELECT * FROM [dbo].[CustomerType]").ToArray();
            int[] regionIds = cn.Query<int>("SELECT [Id] FROM [dbo].[Region]").ToArray();            

            tdg.Generate<Customer>(100, (c) =>
            {
                c.OrganizationId = tdg.Random(customerOrgIds);
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
