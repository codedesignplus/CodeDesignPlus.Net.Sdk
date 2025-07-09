using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Controllers;

[Route("api/[controller]")]
[ApiController]
[Description("This is a fake controller used for testing purposes.")]
public class FakeController : ControllerBase
{

    public IActionResult FakeMethod()
    {
        return Ok("This is a fake method for testing purposes.");
    }

    [HttpOptions]
    public IActionResult CustomMethodWithVerb()
    {
        return Ok("This is a custom method with a verb for testing purposes.");
    }

    [Description("Get method for the fake items.")]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("This is a fake controller for testing purposes.");
    }

    [HttpPost]
    [Description("This method is used to create a new fake item.")]
    public IActionResult Post([FromBody] string value)
    {
        return CreatedAtAction(nameof(Get), new { id = value }, value);
    }

    [HttpPut("{id}")]
    [Description("This method is used to update an existing fake item.")]
    public IActionResult Put(string id, [FromBody] string value)
    {
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Description("This method is used to delete an existing fake item.")]
    public IActionResult Delete(string id)
    {
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Description("This method is used to partially update an existing fake item.")]
    public IActionResult Patch(string id, [FromBody] string value)
    {
        return NoContent();
    }
}


[Route("api/[controller]")]
public class FakeControllerWithNoDescription : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("This is a fake controller with no description for testing purposes.");
    }

    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return CreatedAtAction(nameof(Get), new { id = value }, value);
    }
}