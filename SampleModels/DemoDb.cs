using Postulate.Orm.SqlServer;
using System.Configuration;

namespace Sample.Models
{
    public class DemoDb : SqlServerDb<int>
    {
        public DemoDb() : base("DefaultConnection")
        {
        }

        public DemoDb(Configuration config) : base(config, "DefaultConnection")
        {
        }
    }
}