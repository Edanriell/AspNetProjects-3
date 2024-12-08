using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Airport.Pages.Airport;

[Authorize("CanEnterSecurity")]
public class AirportSecurityModel : PageModel
{
	public void OnGet()
	{
	}
}