using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

/// <summary>
/// Extension methods for converting Result to IActionResult.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an appropriate IActionResult.
    /// </summary>
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return ToErrorResult(result.Error);
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate IActionResult.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return ToErrorResult(result.Error);
    }

    /// <summary>
    /// Converts a Result{T} to CreatedAtAction result on success.
    /// </summary>
    public static IActionResult ToCreatedResult<T>(
        this Result<T> result,
        string actionName,
        object routeValues,
        ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.CreatedAtAction(actionName, routeValues, result.Value);
        }

        return ToErrorResult(result.Error);
    }

    private static IActionResult ToErrorResult(Error error)
    {
        return error.Code switch
        {
            var code when code.Contains("NotFound") => new NotFoundObjectResult(new ProblemDetails
            {
                Status = 404,
                Title = "Not Found",
                Detail = error.Description
            }),
            var code when code.Contains("Validation") => new BadRequestObjectResult(new ProblemDetails
            {
                Status = 400,
                Title = "Validation Error",
                Detail = error.Description
            }),
            var code when code.Contains("Conflict") => new ConflictObjectResult(new ProblemDetails
            {
                Status = 409,
                Title = "Conflict",
                Detail = error.Description
            }),
            var code when code.Contains("Forbidden") => new ObjectResult(new ProblemDetails
            {
                Status = 403,
                Title = "Forbidden",
                Detail = error.Description
            }) { StatusCode = 403 },
            _ => new BadRequestObjectResult(new ProblemDetails
            {
                Status = 400,
                Title = "Error",
                Detail = error.Description
            })
        };
    }
}
