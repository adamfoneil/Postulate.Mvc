using Postulate.Mvc.Interfaces;
using Postulate.Orm.Models;
using Sample.Models;
using SampleWebApp.Queries;
using System.Collections.Generic;

namespace SampleWebApp.ViewModels
{
	public enum SampleEnum
	{
		Jiminy = 1,
		Hambone = 2,
		Thenselvastrum = 3
	}

	public class CustomersView : IQueryTrace
	{
		public SampleEnum SampleEnumValue { get; set; }
		public AllCustomers Query { get; set; }
		public IEnumerable<Customer> Customers { get; set; }

		public bool QueryTraceEnabled { get; set; }
		public IEnumerable<QueryTrace> QueryTraces { get; set; }
	}
}