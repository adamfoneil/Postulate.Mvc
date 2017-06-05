using Postulate.Sql.Abstract;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApp.Queries
{
    public class DemoDbQuery<TResult> : Query<TResult>
    {
        public DemoDbQuery(string sql) : base(sql, () => new DemoDb2().GetConnection())
        {
        }
    }
}