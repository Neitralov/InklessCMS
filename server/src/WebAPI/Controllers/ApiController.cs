namespace WebAPI.Controllers;

/// <inheritdoc/>
[ApiController]
public class ApiController : ControllerBase
{
    /// <summary></summary>>
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var modelStateDictionary = new ModelStateDictionary();

            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(modelStateDictionary);
        }

        if (errors.Any(e => e.Type == ErrorType.Unexpected))
            return Problem();

        var firstError = errors.First();

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            statusCode: statusCode,
            title: firstError.Description);
    }
    
    /// <summary></summary>>
    protected Guid GetUserGuid() => Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}