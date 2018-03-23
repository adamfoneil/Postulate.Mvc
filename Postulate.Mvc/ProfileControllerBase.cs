using Postulate.Orm.Abstract;
using Postulate.Orm.Interfaces;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Postulate.Mvc
{
	public abstract class ProfileControllerBase<TDb, TKey, TProfile> : ControllerBase<TDb, TKey>
		where TProfile : Record<TKey>, IUserProfile, new()
		where TDb : SqlDb<TKey>, new()
	{
		private TProfile _profile = null;

		protected TProfile CurrentUser { get { return _profile; } }

		/// <summary>
		/// Specifies the criteria for a well-formed user profile
		/// </summary>
		protected abstract Func<TProfile, bool> ProfileRule { get; }

		/// <summary>
		/// Indicates where to redirect if current user has no TProfile record or if ProfileRule returns false
		/// </summary>
		protected abstract ActionResult ProfileUpdateRedirect { get; }

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			_profile = Db.FindUserProfile<TProfile>();
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			if (_profile == null || !ProfileRule.Invoke(_profile))
			{
				filterContext.Result = ProfileUpdateRedirect;
			}
		}
	}
}