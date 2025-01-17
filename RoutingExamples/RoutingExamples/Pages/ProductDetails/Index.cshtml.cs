﻿#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoutingExamples.Pages.ProductDetails;

public class IndexModel : PageModel
{
	private readonly ProductService _service;

	public IndexModel(ProductService service)
	{
		_service = service;
	}

	public Product Selected { get; set; }

	public IActionResult OnGet(string name)
	{
		Selected = _service.GetProduct(name);
		if (Selected is null) return NotFound();
		return Page();
	}
}