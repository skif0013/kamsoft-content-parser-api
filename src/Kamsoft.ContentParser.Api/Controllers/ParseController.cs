using Kamsoft.ContentParser.Api.Domain.Models;
using Kamsoft.ContentParser.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kamsoft.ContentParser.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class ParseController : ControllerBase
{
    private readonly ParseService _parseService;

    public ParseController(ParseService parseService)
    {
        _parseService = parseService;
    }

    [HttpPost("parse-content")]
    public IActionResult ParseContent([FromBody] ParseRequest request)
    {
        var result = _parseService.Parse(request);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}
