#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoutingExamples.Pages;

public class SearchModel : PageModel
{
	private readonly LinkGenerator _link;
	private readonly ProductService _productService;

	public SearchModel(ProductService productService, LinkGenerator link)
	{
		_productService = productService;
		_link = link;
	}

	[BindProperty] [Required] public string SearchTerm { get; set; }

	public List<Product> Results { get; private set; }

	public void OnGet()
	{
		// Demonstrates link generation 
		var url1 = Url.Page("ProductDetails/Index", new { name = "big-widget" });
		var url2 = _link.GetPathByPage("/ProductDetails/Index", values: new { name = "big-widget" });
		var url3 = _link.GetPathByPage(HttpContext, "/ProductDetails/Index", values: new { name = "big-widget" });
		var url4 = _link.GetUriByPage(
			"/ProductDetails/Index",
			null,
			new { name = "big-widget" },
			"https",
			new HostString("example.com"));
	}

	public void OnPost()
	{
		if (ModelState.IsValid) Results = _productService.Search(SearchTerm, StringComparison.Ordinal);
	}

	public void OnPostIgnoreCase()
	{
		if (ModelState.IsValid) Results = _productService.Search(SearchTerm, StringComparison.OrdinalIgnoreCase);
	}
}