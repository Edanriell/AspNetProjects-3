﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Models;

namespace RecipeApplication.Pages.Recipes;

public class ViewModel : PageModel
{
	private readonly IAuthorizationService _authService;
	private readonly ILogger<ViewModel> _logger;

	private readonly RecipeService _service;

	public ViewModel(RecipeService service, IAuthorizationService authService, ILogger<ViewModel> logger)
	{
		_service = service;
		_authService = authService;
		_logger = logger;
	}

	public RecipeDetailViewModel Recipe { get; set; }
	public bool CanEditRecipe { get; set; }

	public async Task<IActionResult> OnGetAsync(int id)
	{
		Recipe = await _service.GetRecipeDetail(id);
		if (Recipe is null)
		{
			// If id is not for a valid Recipe, generate a 404 error page
			// TODO: Add status code pages middleware to show friendly 404 page
			_logger.LogWarning(12, "Could not find recipe with id {RecipeId}", id);
			return NotFound();
		}

		var recipe = await _service.GetRecipe(id);
		var isAuthorised = await _authService.AuthorizeAsync(User, recipe, "CanManageRecipe");
		CanEditRecipe = isAuthorised.Succeeded;
		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAsync(int id)
	{
		var recipe = await _service.GetRecipe(id);
		var authResult = await _authService.AuthorizeAsync(User, recipe, "CanManageRecipe");
		if (!authResult.Succeeded) return new ForbidResult();

		await _service.DeleteRecipe(id);

		return RedirectToPage("/Index");
	}
}