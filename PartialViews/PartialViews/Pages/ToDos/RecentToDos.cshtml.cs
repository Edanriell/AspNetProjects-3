using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PartialViews.Pages.ToDos;

public class RecentToDosModel : PageModel
{
	private readonly TaskService _taskService;

	public RecentToDosModel(TaskService taskService)
	{
		_taskService = taskService;
	}

	public int RecentToDosToDisplay { get; } = 3;

	public IEnumerable<ToDoItemViewModel> RecentToDos { get; set; }

	public void OnGet()
	{
		RecentToDos = _taskService.AllTasks.Take(RecentToDosToDisplay);
	}
}