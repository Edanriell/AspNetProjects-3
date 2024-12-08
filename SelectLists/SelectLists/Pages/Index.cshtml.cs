using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SelectLists.Pages;

public class IndexModel : PageModel
{
	private static readonly SelectListGroup _dynamicGroup = new() { Name = "Dynamic" };
	private static readonly SelectListGroup _staticGroup = new() { Name = "Static" };

	//Remove the Group value from the ItemsWithGroups list items
	public ICollection<SelectListItem> Items => ItemsWithGroups
	   .Select(item => new SelectListItem
					   {
						   Value = item.Value,
						   Text = item.Text
					   })
	   .ToList();

	public ICollection<SelectListItem> ItemsWithGroups { get; set; } = new List<SelectListItem>
																	   {
																		   new()
																		   {
																			   Value = "js", Text = "JavaScript",
																			   Group = _dynamicGroup
																		   },
																		   new()
																		   {
																			   Value = "cpp", Text = "C++",
																			   Group = _staticGroup
																		   },
																		   new()
																		   {
																			   Value = "python", Text = "Python",
																			   Group = _dynamicGroup
																		   },
																		   new() { Value = "csharp", Text = "C#" }
																	   };

	[BindProperty] public InputModel Input { get; set; }

	public bool ShowResults { get; set; }

	public void OnGet()
	{
	}

	public void OnPost()
	{
		ShowResults = true;
	}

	public class InputModel
	{
		public string SelectedValue1 { get; set; }
		public string SelectedValue2 { get; set; }
		public IEnumerable<string> MultiValues1 { get; set; } = new List<string>();
		public IEnumerable<string> MultiValues2 { get; set; } = new List<string>();
	}
}