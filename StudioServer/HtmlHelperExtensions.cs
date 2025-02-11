namespace StudioServer;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelperExtensions
{
	public static string IsSelected(this IHtmlHelper html, string page)
	{
		string? currentPage = (string?)html.ViewContext.RouteData.Values["page"];
		return currentPage?.EndsWith(page) == true ? "active" : string.Empty;
	}

	public static string GetSubdomain(this IHtmlHelper html, string subdomain)
	{
		HttpRequest request = html.ViewContext.HttpContext.Request;
		return $"{request.Scheme}://{subdomain}.{request.Host}";
	}
}