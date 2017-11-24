using Postulate.Mvc.Extensions;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
    public static partial class HtmlHelpers
    {
        public static string CurrentAction(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.Request.CurrentAction();
        }

        public static string CurrentController(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.Request.CurrentController();
        }

        public static MvcHtmlString ActionNameField(this HtmlHelper html)
        {
            TagBuilder hidden = HiddenInput("actionName", CurrentAction(html));
            return MvcHtmlString.Create(hidden.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ReturnUrlField(this HtmlHelper html)
        {
            TagBuilder hidden = HiddenInput("returnUrl", html.ViewContext.RequestContext.HttpContext.Request.QueryString["returnUrl"]);
            return MvcHtmlString.Create(hidden.ToString(TagRenderMode.SelfClosing));
        }

        private static TagBuilder HiddenInput(string name, object value)
        {
            TagBuilder hidden = new TagBuilder("input");
            hidden.MergeAttribute("type", "hidden");
            hidden.MergeAttribute("name", name);
            hidden.MergeAttribute("value", value?.ToString());
            return hidden;
        }
    }
}