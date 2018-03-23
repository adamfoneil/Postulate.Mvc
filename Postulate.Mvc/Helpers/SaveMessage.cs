using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
	public enum AlertType
	{
		Success,
		Info,
		Warning,
		Danger
	}

	public static partial class HtmlHelpers
	{
		public static MvcHtmlString SaveMessage(this HtmlHelper html, TempDataDictionary tempData)
		{
			if (tempData.ContainsKey("error"))
			{
				return AlertMessage(html, AlertType.Danger, tempData["error"] as string);
			}

			if (tempData.ContainsKey("success"))
			{
				return AlertMessage(html, AlertType.Success, tempData["success"] as string);
			}

			return MvcHtmlString.Create(string.Empty);
		}

		private static string AlertCssClass(AlertType alertType)
		{
			var values = new Dictionary<AlertType, string>()
			{
				{ AlertType.Success, "alert-success" },
				{ AlertType.Info, "alert-info" },
				{ AlertType.Warning, "alert-warning" },
				{ AlertType.Danger, "alert-danger" }
			};
			return values[alertType];
		}

		public static MvcHtmlString AlertMessage(this HtmlHelper html, AlertType alertType, string message)
		{
			TagBuilder div = AlertMessageDiv(alertType);
			div.SetInnerText(message);
			return MvcHtmlString.Create(div.ToString(TagRenderMode.Normal));
		}

		private static TagBuilder AlertMessageDiv(AlertType alertType)
		{
			TagBuilder div = new TagBuilder("div");
			div.AddCssClass("alert");
			div.AddCssClass(AlertCssClass(alertType));
			return div;
		}

		public static IDisposable BeginAlertMessage(this HtmlHelper html, AlertType alertType)
		{
			var div = AlertMessageDiv(alertType);
			html.ViewContext.Writer.Write(div.ToString(TagRenderMode.StartTag));
			return new Enclosable(html, div);
		}
	}
}