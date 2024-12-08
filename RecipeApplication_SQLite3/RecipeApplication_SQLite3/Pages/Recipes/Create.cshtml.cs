using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Data;
using RecipeApplication.Models;

namespace RecipeApplication.Pages.Recipes;

[Authorize]
public class CreateModel : PageModel
{
	private readonly RecipeService _service;
	private readonly UserManager<ApplicationUser> _userService;

	public CreateModel(RecipeService service, UserManager<ApplicationUser> userService)
	{
		_service = service;
		_userService = userService;
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
				var appUser = await _userService.GetUserAsync(User);
				var id = await _service.CreateRecipe(Input, appUser);
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