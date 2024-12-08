namespace ATypicalRazorPage;

public record ToDoListModel(string Category, string Title);

public class ToDoService
{
	// These would normally be loaded from a database for example
	private static readonly List<ToDoListModel> _items = new()
														 {
															 new("Simple", "Bread"),
															 new("Simple", "Milk"),
															 new("Simple", "Get Gas"),
															 new("Long", "Write Book"),
															 new("Long", "Build Application")
														 };

	public List<ToDoListModel> GetItemsForCategory(string category)
	{
		// filter by the provided category
		return _items.Where(x => x.Category == category).ToList();
	}
}