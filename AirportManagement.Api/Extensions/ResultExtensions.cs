using AirportManagement.Application;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Results;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.Api.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult(this ControllerBase controller, Result result)
            => result.Status switch
            {
                ResultStatus.Ok => controller.Ok(result),

                ResultStatus.Invalid => controller.BadRequest(result),

                ResultStatus.NotFound => controller.NotFound(result),

                ResultStatus.Conflict => controller.Conflict(result),

                ResultStatus.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, result),
                _ => controller.BadRequest(result)
            };

        public static ActionResult ToCreatedResult<T>(this ControllerBase controller, ResultObject<T> result)
            => result.Status switch
            {
                ResultStatus.Ok => controller.StatusCode(StatusCodes.Status201Created, result),

                ResultStatus.Invalid => controller.BadRequest(result),

                ResultStatus.NotFound => controller.NotFound(result),

                ResultStatus.Conflict => controller.Conflict(result),

                ResultStatus.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, result),
                _ => controller.BadRequest(result)
            };
    }
}
