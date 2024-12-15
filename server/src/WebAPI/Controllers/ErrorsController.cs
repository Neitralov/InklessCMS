namespace WebAPI.Controllers;

/// <inheritdoc />
public sealed class ErrorsController : ControllerBase
{
    /// <summary></summary>>
    [HttpGet("error"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error() => Problem();
}