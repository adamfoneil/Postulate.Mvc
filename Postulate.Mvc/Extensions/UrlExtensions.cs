using System.Web;
using System.Web.Mvc;

namespace Postulate.Mvc.Extensions
{
    public static class UrlExtensions
	{
		public static string Base(this UrlHelper url, string append = null)
		{
			// thanks to http://stackoverflow.com/questions/1288046/how-can-i-get-my-webapps-base-url-in-asp-net-mvc
			var request = url.RequestContext.HttpContext.Request;
			string appUrl = HttpRuntime.AppDomainAppVirtualPath;
			if (appUrl != "/") appUrl = "/" + appUrl;
			return $"{request.Url.Scheme}://{request.Url.Authority}{appUrl}{append}";
		}
	}
}