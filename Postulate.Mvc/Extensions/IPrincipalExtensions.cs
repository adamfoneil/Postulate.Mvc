using Postulate.Mvc.Abstract;
using System.Security.Principal;
using System.Web;

namespace Postulate.Mvc.Extensions
{
	public static class IPrincipalExtensions
	{
		public static T LoadUserData<T>(this IPrincipal user, HttpServerUtilityBase server) where T : UserData, new()
		{
			return UserData.Load<T>(server, user.Identity.Name);
		}
	}
}