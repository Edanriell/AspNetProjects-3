using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Models;

namespace RecipeApplication.Pages;

public class IndexModel : PageModel
{
	private readonly RecipeService _service;

	public IndexModel(RecipeService service)
	{
		_service = service;
	}

	public IEnumerable<RecipeSummaryViewModel> Recipes { get; private set; }

	public async Task OnGet()
	{
		Recipes = await _service.GetRecipes();
	}
}