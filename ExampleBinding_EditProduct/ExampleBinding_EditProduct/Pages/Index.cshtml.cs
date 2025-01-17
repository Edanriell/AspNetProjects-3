﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleBinding.Pages;

[IgnoreAntiforgeryToken] // So you can call the page from 
public class IndexModel : PageModel
{
	public ProductModel Product { get; set; }

	public RedirectToPageResult OnGet()
	{
		return RedirectToPage("EditProduct");
	}

	public void OnPostEditProduct(ProductModel product)
	{
		Product = product;
	}

	public void OnPostEditTwoProducts(ProductModel product1, ProductModel product2)
	{
		Product = product1;
	}
}