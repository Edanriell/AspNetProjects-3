using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ToDoList.Pages;

public class SimpleViewModel : PageModel
{
	private readonly ToDoService _service;

	public SimpleViewModel(ToDoService service)
	{
		_service = service;
	}

	public ToDoItem ToDo { get; set; }

	public IActionResult OnGet(int id)
	{
		ToDo = _service.AllItems.FirstOrDefault(x => x.Id == id);
		if (ToDo == null) return RedirectToPage("Index");
		return Page();
	}
}