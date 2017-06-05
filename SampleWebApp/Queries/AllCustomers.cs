using Postulate.Sql.Abstract;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApp.Queries
{
    public class AllCustomers : DemoDbQuery<Customer>
    {
        public AllCustomers() : base("SELECT * FROM [Customer] ORDER BY [LastName], [FirstName]")
        {
        }
    }
}