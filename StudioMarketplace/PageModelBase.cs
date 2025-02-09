namespace StudioMarketplace;

using Microsoft.AspNetCore.Mvc.RazorPages;

public abstract class PageModelBase : PageModel
{
	protected readonly ILogger Log;

	public PageModelBase(ILogger log)
	{
		this.Log = log;
	}
}
