using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATypicalRazorPage;

public class CategoryModel : PageModel
{
	private readonly ToDoService _service;

	public CategoryModel(ToDoService service)
	{
		_service = service;
	}
	//  The ToDoService is provided in the model constructor using DI.

	public List<ToDoListModel> Items { get; set; }
	// The Razor View can access the Items property when it’s rendered.

	// The OnGet handler takes a parameter, category
	public ActionResult OnGet(string category)
	{
		Items = _service.GetItemsForCategory(category);
		// The handler calls out to the ToDoService to retrieve data and sets the Items property.
		return Page();
		// Returns a PageResult indicating the Razor view should be rendered
	}
}