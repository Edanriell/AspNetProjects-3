using Microsoft.AspNetCore.Mvc;

namespace ApiControllerAttributeExample.Controllers;

[ApiController]
[Route("[controller]")]
public class FruitController : ControllerBase
{
	private readonly List<string> _fruit = new()
										   {
											   "Pear", "Lemon", "Peach"
										   };

	[HttpGet]
	public ActionResult Update()
	{
		return Ok(_fruit);
	}

	[HttpPost]
	public ActionResult Update(UpdateModel model)
	{
		if (model.Id < 0 || model.Id > _fruit.Count) return NotFound();

		_fruit[model.Id] = model.Name;
		return Ok();
	}
}