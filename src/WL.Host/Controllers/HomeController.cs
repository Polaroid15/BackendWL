using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WL.Host.Exceptions;

namespace WL.Host.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Handle unobserved exceptions.
    /// </summary>
    /// <param name="configuration">Service configuration.</param>
    /// <returns>JSON response by error.</returns>
    [Route("/error")]
    public IActionResult Error([FromServices] IConfiguration configuration)
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        int statusCode;
        string title;
        var fullError = configuration.GetSection("AllowFullError").Get<bool>() ? context?.Error.ToString() : null;

        switch (context?.Error)
        {
            case ArgumentException ae:
                statusCode = 400;
                title = ae.Message + $"(param: {ae.ParamName})";
                break;
            case UnauthorizedException uh:
                statusCode = 401;
                title = $"Unauthorized, {uh.Message}";
                break;
            case NotSupportedException nse:
                statusCode = 405;
                title = "Method Not Allowed";
                break;
            case NotImplementedException nie:
                statusCode = 501;
                title = "Not Implemented";
                break;
            default:
                statusCode = 500;
                title = "Internal Server Error";
                break;
        }

        return Problem(
            title: title,
            statusCode: statusCode,
            detail: fullError);
    }
}