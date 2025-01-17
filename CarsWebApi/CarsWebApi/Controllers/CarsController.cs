﻿using Microsoft.AspNetCore.Mvc;

namespace CarsWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
    /// <summary>
    ///     This represents the global application model that would
    ///     normally be stored in a database etc
    /// </summary>
    private static readonly List<Car> Cars = new();

	private readonly string _carsAsXml = "<cars><car>Nissan Micra</car><car>Ford Focus</car></cars>";

	[HttpGet("start")]
	[HttpGet("ignition")]
	[HttpGet("/start-car")]
	public IEnumerable<string> ListCars()
	{
		return new[]
			   { "Nissan Micra", "Ford Focus" };
	}

	[HttpGet("null")]
	public IActionResult Null()
	{
		return Ok(null);
	}

	[HttpGet("content")]
	public string ListCarsAsContent()
	{
		return _carsAsXml;
	}

	[HttpGet("xml")]
	public IActionResult ListCarsAsXml()
	{
		return Content(_carsAsXml, "text/xml");
	}

	[HttpGet("json")]
	public IActionResult ListCarsAsJson()
	{
		return new JsonResult(new[]
							  { "Nissan Micra", "Ford Focus" });
	}

	[HttpPost]
	public IActionResult Add(Car car)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		Cars.Add(car);
		return Ok();
	}
}