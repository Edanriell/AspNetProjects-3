using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Models;

namespace RecipeApplication.Pages;

public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly RecipeService _service;

	public IndexModel(RecipeService service, ILogger<IndexModel> logger)
	{
		_service = service;
		_logger = logger;
	}

	public ICollection<RecipeSummaryViewModel> Recipes { get; private set; }

	public async Task OnGet()
	{
		Recipes = await _service.GetRecipes();
		_logger.LogInformation("Loaded {RecipeCount} recipes", Recipes.Count);
	}
}