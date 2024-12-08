using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Models;

namespace RecipeApplication.Pages.Recipes;

public class ViewModel : PageModel
{
	private readonly RecipeService _service;

	public ViewModel(RecipeService service)
	{
		_service = service;
	}

	public RecipeDetailViewModel Recipe { get; set; }

	public async Task<IActionResult> OnGetAsync(int id)
	{
		Recipe = await _service.GetRecipeDetail(id);
		if (Recipe is null)
			// If id is not for a valid Recipe, generate a 404 error page
			// TODO: Add status code pages middleware to show friendly 404 page
			return NotFound();
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(int id)
	{
		await _service.DeleteRecipe(id);

		return RedirectToPage("/Index");
	}
}