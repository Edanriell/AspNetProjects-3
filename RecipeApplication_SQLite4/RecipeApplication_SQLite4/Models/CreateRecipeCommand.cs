﻿using RecipeApplication.Data;

namespace RecipeApplication.Models;

public class CreateRecipeCommand : EditRecipeBase
{
	public IList<CreateIngredientCommand> Ingredients { get; set; } = new List<CreateIngredientCommand>();

	public Recipe ToRecipe(ApplicationUser createdBy)
	{
		return new Recipe
			   {
				   Name = Name,
				   TimeToCook = new TimeSpan(TimeToCookHrs, TimeToCookMins, 0),
				   Method = Method,
				   IsVegetarian = IsVegetarian,
				   IsVegan = IsVegan,
				   LastModified = DateTime.UtcNow,
				   CreatedById = createdBy.Id,
				   Ingredients = Ingredients?.Select(x => x.ToIngredient()).ToList()
			   };
	}
}