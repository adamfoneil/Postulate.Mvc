using Postulate.Orm;
using Sample.Models;

namespace SampleWebApp.Queries
{
    public class DemoDbQuery<TResult> : Query<TResult>
    {
        public DemoDbQuery(string sql) : base(sql, new DemoDb())
        {
        }
    }
}