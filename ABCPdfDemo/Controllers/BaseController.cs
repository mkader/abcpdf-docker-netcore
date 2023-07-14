using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ABCPdf.Demo.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected const string _problemJsonContentType = "application/problem+json";

        protected ObjectResult ExceptionProblemDetails(Exception ex, HttpContext context,
            int statusCode = StatusCodes.Status500InternalServerError)
        {
            return ErrorProblmDetails(ex.Message, context,statusCode, ex.InnerException?.Message);
        }

        protected ObjectResult NotFoundProblemDetails(string detail, HttpContext context)
        {
            return ProblemDetails(StatusCodes.Status404NotFound, "Object Not Found", detail, context);
        }

        protected ObjectResult ConflictProblemDetails(string detail, HttpContext context)
        {
            return ProblemDetails(StatusCodes.Status409Conflict, "Conflict", detail, context);
        }

        protected ObjectResult BadRequestProblemDetails(string detail, HttpContext context)
        {
            return ProblemDetails(StatusCodes.Status400BadRequest, "Bas Request", detail, context);
        }

        protected ObjectResult ErrorProblmDetails(string title, HttpContext context,
            int statusCode = StatusCodes.Status500InternalServerError, string? detail = null)
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
                statusCode = StatusCodes.Status500InternalServerError;
            return ProblemDetails(statusCode, title, detail, context);
        }

        private static ObjectResult ProblemDetails(int statusCode, string title, string detail, HttpContext context)
        {
            var details = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context?.Request?.Path
            };

            return new NotFoundObjectResult(details)
            {
                ContentTypes = { _problemJsonContentType },
                StatusCode = statusCode
            };
        }
    }
}
