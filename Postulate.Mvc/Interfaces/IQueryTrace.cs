using Postulate.Orm.Models;
using System.Collections.Generic;

namespace Postulate.Mvc.Interfaces
{
	public interface IQueryTrace
	{
		bool QueryTraceEnabled { get; set; }
		IEnumerable<QueryTrace> QueryTraces { get; set; }
	}
}