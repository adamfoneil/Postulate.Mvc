using Postulate.Orm;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using Postulate.Orm.Abstract;
using System;
using Postulate.Orm.Exceptions;

namespace Postulate.Mvc
{
    public class SqlServerDbController<TDb, TKey> : Controller where TDb : SqlServerDb<TKey>, new()
    {
        protected SqlServerDb<TKey> _db = new TDb();

        protected override void Initialize(RequestContext requestContext)
        {            
            base.Initialize(requestContext);
            _db.UserName = User.Identity.Name;
        }

        protected bool SaveRecord<TRecord>(TRecord record) where TRecord : Record<TKey>
        {
            try
            {
                _db.Save(record);
                return true;
            }
            catch (Exception exc)
            {
                CaptureErrorMessage(exc);
                return false;
            }            
        }

        protected void CaptureErrorMessage(Exception exc)
        {
            if (TempData.ContainsKey("error")) TempData.Remove("error");
            TempData.Add("error", exc.Message);

            SaveException se = exc as SaveException;
            if (se != null)
            {
                TempData.Add("command", se.CommandText);
                //TempData.Add("params", se.Par);
            }
        }
    }
}
