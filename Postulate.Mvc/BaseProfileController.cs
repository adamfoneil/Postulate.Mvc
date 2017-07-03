using Postulate.Orm;
using System.Web.Mvc;
using System.Web.Routing;
using Postulate.Orm.Abstract;
using System;
using Postulate.Orm.Exceptions;
using System.Data;
using Dapper;
using System.Linq;
using System.Reflection;
using Postulate.Orm.Interfaces;
using System.Collections.Generic;

namespace Postulate.Mvc
{
    public abstract class BaseProfileController<TDb, TKey, TProfile> : BaseController<TDb, TKey>        
        where TProfile : Record<TKey>, IUserProfile
        where TDb : SqlServerDb<TKey>, new()
    {        
        private TProfile _profile = null;
        
        protected TProfile CurrentUser { get { return _profile; } }

        /// <summary>
        /// Specifies what a well-formed profile must have
        /// </summary>
        protected Func<TProfile, bool> ProfileRule;

        /// <summary>
        /// Indicates where to redirect if current user has no TProfile record
        /// </summary>
        protected abstract string ProfileUpdateUrl { get; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);            
            _profile = Db.FindUserProfile<TProfile>();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {            
            base.OnActionExecuting(filterContext);

            if (_profile == null || (_profile != null && (!
                ProfileRule?.Invoke(_profile) ?? true)))
            {
                // thanks to https://stackoverflow.com/questions/32925219/how-to-create-a-custom-attribute-that-will-redirect-to-login-if-it-returns-false
                filterContext.Result = new RedirectResult(ProfileUpdateUrl);
            }
        }
    }
}
