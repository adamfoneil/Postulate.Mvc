using Postulate.Orm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Sample.Models
{
    public class DemoDb2 : SqlServerDb<int>
    {
        public DemoDb2() : base("DefaultConnection")
        {
        }

        public DemoDb2(Configuration config) : base(config, "DefaultConnection")
        {
        }
    }
}