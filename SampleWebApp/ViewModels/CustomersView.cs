using Postulate.Mvc.Interfaces;
using Postulate.Orm.Models;
using Sample.Models;
using SampleWebApp.Queries;
using System.Collections.Generic;

namespace SampleWebApp.ViewModels
{
	public class CustomersView : IQueryTrace
	{
		public AllCustomers Query { get; set; }
		public IEnumerable<Customer> Customers { get; set; }

		public bool QueryTraceEnabled { get; set; }
		public IEnumerable<QueryTrace> QueryTraces { get; set; }
	}
}