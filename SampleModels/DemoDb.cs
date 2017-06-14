using Postulate.Orm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

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