using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
	public static partial class HtmlHelpers
	{
		private const string pageField = "page";

		public static string NextPageHref(this HtmlHelper html)
		{
			return PagingHref(html, 1);
		}

		public static string PreviousPageHref(this HtmlHelper html)
		{
			return PagingHref(html, -1);
		}

		// help from https://stackoverflow.com/a/39502422/2023653
		private static string PagingHref(HtmlHelper html, int offset)
		{
			var request = html.ViewContext.RequestContext.HttpContext.Request;
			NameValueCollection values = HttpUtility.ParseQueryString(string.Empty);
			values.Add(request.QueryString);
			values[pageField] = (CurrentPage(request) + offset).ToString();
			return "?" + values.ToString();
		}

		public static int CurrentPage(this HtmlHelper html)
		{
			var request = html.ViewContext.RequestContext.HttpContext.Request;
			return CurrentPage(request);
		}

		private static int CurrentPage(HttpRequestBase request)
		{
			string page = request[pageField];
			if (!string.IsNullOrEmpty(page))
			{
				int result;
				if (int.TryParse(page, out result)) return result;
			}

			return 1;
		}
	}
}