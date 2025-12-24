using AirportManagement.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application
{
    public class Result
    {
        public ResultStatus Status { get; }
        public string? Error { get; }

        public bool IsSuccess => Status == ResultStatus.Ok;

        public Result(ResultStatus status, string? error)
        {
            Status = status;
            Error = error;
        }

        public static Result Success() => new(ResultStatus.Ok, null);

        public static Result NotFound(string? error = null) => new(ResultStatus.NotFound, error);

        public static Result Invalid(string error) => new(ResultStatus.Invalid, error);

        public static Result Conflict(string error) => new(ResultStatus.Conflict, error);
        public static Result Forbidden(string error) => new(ResultStatus.Forbidden, error);
    }
}
