using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageUsers.Pages;

public class ViewUserModel : PageModel
{
	[BindProperty(SupportsGet = true)] public string Username { get; set; }

	public void OnGet()
	{
	}
}