using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Models;

namespace RecipeApplication.Pages.Recipes;

public class CreateModel : PageModel
{
	private readonly RecipeService _service;

	public CreateModel(RecipeService service)
	{
		_service = service;
	}

	[BindProperty] public CreateRecipeCommand Input { get; set; }

	public void OnGet()
	{
		Input = new CreateRecipeCommand();
	}

	public async Task<IActionResult> OnPost()
	{
		try
		{
			if (ModelState.IsValid)
			{
				var id = await _service.CreateRecipe(Input);
				return RedirectToPage("View", new { id });
			}
		}
		catch (Exception)
		{
			// TODO: Log error
			// Add a model-level error by using an empty string key
			ModelState.AddModelError(
					string.Empty,
					"An error occured saving the recipe"
				);
		}

		//If we got to here, something went wrong
		return Page();
	}
}