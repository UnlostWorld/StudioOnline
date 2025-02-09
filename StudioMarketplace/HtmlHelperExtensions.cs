﻿namespace StudioMarketplace;

using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelperExtensions
{
	public static string IsSelected(this IHtmlHelper html, string? page)
	{
		var currentPage = (string?)html.ViewContext.RouteData.Values["page"];
		return page == currentPage  ? "active" : string.Empty;
	}
}