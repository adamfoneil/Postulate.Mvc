using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Postulate.Mvc
{
    public static class HtmlHelpers
    {
        public static string ActionName(this HtmlHelper html)
        {
            return html.ViewContext.RequestContext.RouteData.Values["action"] as string;
        }

        public static MvcHtmlString ActionNameField(this HtmlHelper html)
        {
            TagBuilder hidden = HiddenInput("actionName", ActionName(html));            
            return MvcHtmlString.Create(hidden.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ReturnUrlField(this HtmlHelper html)
        {
            TagBuilder hidden = HiddenInput("returnUrl", html.ViewContext.RequestContext.HttpContext.Request.RawUrl);            
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
