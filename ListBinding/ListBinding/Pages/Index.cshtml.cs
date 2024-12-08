using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ListBinding.Pages;

public class IndexModel : PageModel
{
	private readonly CurrencyService _service;

	public IndexModel(CurrencyService service)
	{
		_service = service;
	}

	public List<SelectListItem> Currencies { get; set; }

	public void OnGet()
	{
		Currencies = _service.AllCurrencies
		   .Select(x => new SelectListItem { Text = x.Key })
		   .ToList();
	}
}