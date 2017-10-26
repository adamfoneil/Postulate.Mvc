using System;
using System.Web.Mvc;

namespace Postulate.Mvc.Extensions
{
    public static class HtmlHelpers
    {
        public static string CurrentAction(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.Request.CurrentAction();
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

        public static MvcHtmlString YesNoDropdown(this HtmlHelper html, string fieldName, bool? value, string yesText = "Yes", string noText = "No", object htmlAttributes = null)
        {
            TagBuilder select = new TagBuilder("select");
            select.MergeAttribute("name", fieldName);
            select.MergeAttribute("id", fieldName);
            if (htmlAttributes != null) select.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            select.InnerHtml = 
                BlankOption() + 
                YesNoDropdownOption(true, value, yesText) + 
                YesNoDropdownOption(false, value, noText);

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        private static string BlankOption()
        {
            TagBuilder option = new TagBuilder("option");
            option.SetInnerText("(select)");
            return option.ToString(TagRenderMode.Normal);
        }

        private static string YesNoDropdownOption(bool optionValue, bool? boundValue, string text)
        {
            TagBuilder option = new TagBuilder("option");
            option.MergeAttribute("value", optionValue.ToString().ToLower());
            option.SetInnerText(text);
            if (boundValue?.Equals(optionValue) ?? false) option.MergeAttribute("selected", "true");
            return option.ToString(TagRenderMode.Normal);
        }
    }
}
