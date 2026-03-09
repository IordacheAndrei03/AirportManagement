using AirportManagement.Application.Enums;
using AirportManagement.Application.Results;

namespace AirportManagement.Application
{
    public class ResultObject<T> : Result
    {
        public T? Value { get; }

        public ResultObject(ResultStatus status, T? value, string? error) : base(status, error)
        {
            Value = value;
        }

        public static ResultObject<T> Success(T value) =>
            new(ResultStatus.Ok, value, null);

        public static new ResultObject<T> NotFound(string? error = null) =>
            new(ResultStatus.NotFound, default, error);

        public static new ResultObject<T> Invalid(string error) =>
            new(ResultStatus.Invalid, default, error);

        public static new ResultObject<T> Conflict(string error) =>
            new(ResultStatus.Conflict, default, error);

        public static new ResultObject<T> Forbidden(string error) =>
            new(ResultStatus.Forbidden, default, error);
    }
}
