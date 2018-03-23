using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
	public static partial class HtmlHelpers
	{
		public static MvcHtmlString YesNoDropdown(this HtmlHelper html, string fieldName, bool? value, string nullText = "(select)", string yesText = "Yes", string noText = "No", object htmlAttributes = null)
		{
			TagBuilder select = new TagBuilder("select");
			select.MergeAttribute("name", fieldName);
			select.MergeAttribute("id", fieldName);
			if (htmlAttributes != null) select.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

			select.InnerHtml =
				BlankOption(nullText) +
				YesNoDropdownOption(true, value, yesText) +
				YesNoDropdownOption(false, value, noText);

			return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
		}

		public static MvcHtmlString YesNoDropdown(this HtmlHelper html, string fieldName, bool? value, string yesText = "Yes", string noText = "No", object htmlAttributes = null)
		{
			return YesNoDropdown(html, fieldName, value, "(select)", yesText, noText, htmlAttributes);
		}

		private static string BlankOption(string text)
		{
			TagBuilder option = new TagBuilder("option");
			option.SetInnerText(text);
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