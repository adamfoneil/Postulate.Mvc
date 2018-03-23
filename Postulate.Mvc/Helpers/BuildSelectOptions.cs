using System.Linq;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
	public static partial class HtmlHelpers
	{
		public static void BuildSelectListOptions(this TagBuilder selectTag, MultiSelectList selectList, object selectedValue)
		{
			string displayText;
			BuildSelectListOptions(selectTag, selectList, null, selectedValue, out displayText);
		}

		public static void BuildSelectListOptions(this TagBuilder selectTag, MultiSelectList selectList, string blankOption, object selectedValue)
		{
			string displayText;
			BuildSelectListOptions(selectTag, selectList, blankOption, selectedValue, out displayText);
		}

		public static void BuildSelectListOptions(this TagBuilder selectTag, MultiSelectList selectList, string blankOption, object selectedValue, out string displayValue)
		{
			if (blankOption != null)
			{
				TagBuilder blank = new TagBuilder("option");
				blank.MergeAttribute("value", string.Empty);
				blank.SetInnerText(blankOption);
				selectTag.InnerHtml += blank.ToString(TagRenderMode.Normal);
			}

			displayValue = null;
			foreach (var item in selectList)
			{
				TagBuilder option = new TagBuilder("option");
				option.MergeAttribute("value", item.Value);
				if (selectedValue != null && IsSelectedValue(selectList, item, selectedValue))
				{
					option.MergeAttribute("selected", "true");
					displayValue = item.Text;
				}
				option.SetInnerText(item.Text);
				selectTag.InnerHtml += option.ToString(TagRenderMode.Normal);
			}
		}

		private static bool IsSelectedValue(MultiSelectList selectList, SelectListItem item, object selectedValue)
		{
			if (selectedValue == null) return false;

			if (selectList is SelectList)
			{
				if (selectedValue.ToString().ToLower().Equals(item.Value.ToLower())) return true;
			}

			if (selectList is MultiSelectList)
			{
				string[] values = selectedValue.ToString().Split(',');
				if (values.Any(s => s.ToLower().Equals(item.Value.ToLower()))) return true;
			}

			return false;
		}
	}
}