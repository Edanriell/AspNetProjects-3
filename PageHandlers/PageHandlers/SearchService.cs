namespace PageHandlers;

public class SearchService
{
	// These would normally be loaded from a database for example
	private static readonly List<Product> _items = new()
												   {
													   new() { Name = "iPad" },
													   new() { Name = "iPod" },
													   new() { Name = "iMac" },
													   new() { Name = "Mac Pro" },
													   new() { Name = "Mac mini" }
												   };

	public List<Product> SearchProducts(string term)
	{
		// filter by the provided category
		return _items.Where(x => x.Name.Contains(term)).ToList();
	}
}