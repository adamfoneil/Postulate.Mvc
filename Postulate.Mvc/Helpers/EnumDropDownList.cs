using System;
using System.Linq;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
	public static partial class HtmlHelpers
	{
		public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper html, string fieldName, TEnum selectedValue, object htmlAttributes = null)
		{
			TagBuilder select = new TagBuilder("select");
			select.MergeAttribute("name", fieldName);
			select.MergeAttribute("id", fieldName);

			var values = (int[])Enum.GetValues(typeof(TEnum));
			var names = Enum.GetNames(typeof(TEnum));
			var items = values.Select((item, index) => new SelectListItem() { Value = item.ToString(), Text = names[index] });
			int intValue = Convert.ToInt32(selectedValue);
			var selectList = new SelectList(items, "Value", "Text", intValue);
			select.BuildSelectListOptions(selectList, null, intValue);

			if (htmlAttributes != null) select.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

			return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
		}
	}
}