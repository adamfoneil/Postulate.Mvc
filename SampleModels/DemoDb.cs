using Postulate.Orm.SqlServer;
using Postulate.Orm.Util;
using System.Configuration;

namespace Sample.Models
{
    public class DemoDb : SqlServerDb<int>
    {
        public DemoDb() : base("DefaultConnection")
        {
            TraceCallback = (cn, qt) => QueryUtil.SaveTrace(cn, qt, this);
        }

        public DemoDb(Configuration config) : base(config, "DefaultConnection")
        {
        }        
    }
}