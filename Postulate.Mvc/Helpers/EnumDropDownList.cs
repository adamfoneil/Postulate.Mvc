using System;
using System.Linq;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
    public static partial class HtmlHelpers
    {
        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper html, TEnum selectedValue, string blankOption, object htmlAttributes = null)
        {
            TagBuilder select = new TagBuilder("select");

            var values = (int[])Enum.GetValues(typeof(TEnum));
            var names = Enum.GetNames(typeof(TEnum));
            var items = values.Select((item, index) => new SelectListItem() { Value = item.ToString(), Text = names[index] });
            var selectList = new SelectList(items, "Value", "Text", selectedValue);
            select.BuildSelectListOptions(selectList, blankOption, selectedValue);

            if (htmlAttributes != null) select.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }
    }
}