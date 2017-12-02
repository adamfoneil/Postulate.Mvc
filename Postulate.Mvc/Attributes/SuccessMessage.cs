using System.Web.Mvc;

namespace Postulate.Mvc.Attributes
{
    /// <summary>
    /// Apply this to controller actions to add a message on successful completion of an operation.
    /// Use Html.SaveMessage(TempData) in Razor views to access the bootstrap-formatted message
    /// </summary>
    public class SuccessMessage : ActionFilterAttribute
    {
        private readonly string _message;

        public SuccessMessage(string message)
        {
            _message = message;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            const string key = "success";
            var tempData = filterContext.Controller.TempData;
            if (tempData.ContainsKey(key)) tempData.Remove(key);
            tempData.Add(key, _message);
        }
    }
}