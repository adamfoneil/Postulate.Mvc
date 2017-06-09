using System.Web;

namespace Postulate.Mvc.Extensions
{
	public static class RequestExtensions
	{
		public static string CurrentAction(this HttpRequestBase request)
		{
			return request.RequestContext.RouteData.Values["action"].ToString();
		}

		public static string CurrentController(this HttpRequestBase request)
		{
			return request.RequestContext.RouteData.Values["controller"].ToString();
		}
	}
}