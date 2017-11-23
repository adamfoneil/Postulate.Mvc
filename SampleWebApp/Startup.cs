using Microsoft.Owin;
using Owin;
using Postulate.Orm.Merge;
using Postulate.Orm.SqlServer;
using Sample.Models;
using System;

[assembly: OwinStartupAttribute(typeof(SampleWebApp.Startup))]
namespace SampleWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            new DemoDb().CreateIfNotExists((cn) =>
            {
                var eng = new Engine<SqlServerSyntax>(new Type[]
                {
                    typeof(Customer), typeof(CustomerType), typeof(Organization), typeof(Region), typeof(UserProfile)
                }, new Progress<MergeProgress>(ShowProgress));
                eng.ExecuteAsync(cn).Wait();
            });
        }

        private void ShowProgress(MergeProgress obj)
        {
            // do nothing
        }
    }
}
