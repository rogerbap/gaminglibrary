// src/Core/GamingLibrary.Application/Common/Models/Result.cs
// Purpose: Result pattern for error handling without exceptions
namespace GamingLibrary.Application.Common.Models
{
    /// <summary>
    /// Result pattern implementation for handling success/failure scenarios
    /// </summary>
    public class Result
    {
        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
        public static Result<T> Failure<T>(string error) => new(default, false, error);
    }

    /// <summary>
    /// Result with a value
    /// </summary>
    public class Result<T> : Result
    {
        private readonly T? _value;

        protected internal Result(T? value, bool isSuccess, string error) : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access value of failed result");

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
