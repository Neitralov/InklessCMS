namespace WebAPI.Controllers;

public sealed class ErrorsController : ControllerBase
{
    [HttpGet("error"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error() => Problem();
}
