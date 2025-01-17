﻿using Microsoft.EntityFrameworkCore;
using RecipeApplication.Data;
using RecipeApplication.Models;

namespace RecipeApplication;

public class RecipeService
{
	private readonly AppDbContext _context;
	private readonly ILogger _logger;

	public RecipeService(AppDbContext context, ILoggerFactory factory)
	{
		_context = context;
		_logger = factory.CreateLogger<RecipeService>();
	}

	public async Task<List<RecipeSummaryViewModel>> GetRecipes()
	{
		return await _context.Recipes
				  .Where(r => !r.IsDeleted)
				  .Select(x => new RecipeSummaryViewModel
							   {
								   Id = x.RecipeId,
								   Name = x.Name,
								   TimeToCook = $"{x.TimeToCook.Hours}hrs {x.TimeToCook.Minutes}mins"
							   })
				  .ToListAsync();
	}

	public async Task<Recipe> GetRecipe(int recipeId)
	{
		return await _context.Recipes
				  .Where(x => x.RecipeId == recipeId)
				  .SingleOrDefaultAsync();
	}

	public async Task<bool> DoesRecipeExistAsync(int id)
	{
		return await _context.Recipes
				  .Where(r => !r.IsDeleted)
				  .Where(r => r.RecipeId == id)
				  .AnyAsync();
	}

	public async Task<RecipeDetailViewModel> GetRecipeDetail(int id)
	{
		return await _context.Recipes
				  .Where(x => x.RecipeId == id)
				  .Where(x => !x.IsDeleted)
				  .Select(x => new RecipeDetailViewModel
							   {
								   Id = x.RecipeId,
								   Name = x.Name,
								   Method = x.Method,
								   LastModified = x.LastModified,
								   Ingredients = x.Ingredients
									  .Select(item => new RecipeDetailViewModel.Item
													  {
														  Name = item.Name,
														  Quantity = $"{item.Quantity} {item.Unit}"
													  })
							   })
				  .SingleOrDefaultAsync();
	}


	public async Task<UpdateRecipeCommand> GetRecipeForUpdate(int recipeId)
	{
		return await _context.Recipes
				  .Where(x => x.RecipeId == recipeId)
				  .Where(x => !x.IsDeleted)
				  .Select(x => new UpdateRecipeCommand
							   {
								   Name = x.Name,
								   Method = x.Method,
								   TimeToCookHrs = x.TimeToCook.Hours,
								   TimeToCookMins = x.TimeToCook.Minutes,
								   IsVegan = x.IsVegan,
								   IsVegetarian = x.IsVegetarian
							   })
				  .SingleOrDefaultAsync();
	}

	/// <summary>
	///     Create a new recipe
	/// </summary>
	/// <param name="cmd"></param>
	/// <returns>The id of the new recipe</returns>
	public async Task<int> CreateRecipe(CreateRecipeCommand cmd, ApplicationUser createdBy)
	{
		var recipe = cmd.ToRecipe(createdBy);
		_context.Add(recipe);
		await _context.SaveChangesAsync();
		return recipe.RecipeId;
	}

	/// <summary>
	///     Updateds an existing recipe
	/// </summary>
	/// <param name="cmd"></param>
	/// <returns>The id of the new recipe</returns>
	public async Task UpdateRecipe(UpdateRecipeCommand cmd)
	{
		var recipe = await _context.Recipes.FindAsync(cmd.Id);
		if (recipe == null) throw new Exception("Unable to find the recipe");
		if (recipe.IsDeleted) throw new Exception("Unable to update a deleted recipe");

		cmd.UpdateRecipe(recipe);
		await _context.SaveChangesAsync();
	}

	/// <summary>
	///     Marks an existing recipe as deleted
	/// </summary>
	/// <param name="cmd"></param>
	/// <returns>The id of the new recipe</returns>
	public async Task DeleteRecipe(int recipeId)
	{
		var recipe = await _context.Recipes.FindAsync(recipeId);
		if (recipe is null) throw new Exception("Unable to find recipe");

		recipe.IsDeleted = true;
		await _context.SaveChangesAsync();
	}
}