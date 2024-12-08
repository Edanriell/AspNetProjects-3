using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ToDoList.Pages;

public class IndexModel : PageModel
{
	private readonly ToDoService _service;

	public IndexModel(ToDoService service)
	{
		_service = service;
	}

	public IEnumerable<ToDoItem> Items { get; set; }

	public void OnGet()
	{
		Items = _service.AllItems;
	}
}