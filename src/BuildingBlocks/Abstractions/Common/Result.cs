namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  public class Result
  {
    public bool IsSuccess { get; }
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
      IsSuccess = isSuccess;
      Error = error;
    }

    public static Result Ok() => new(true, Error.None);
    public static Result Fail(string code, string message) => new(false, new Error(code, message));
  }

  public sealed class Result<T> : Result
  {
    public T? Value { get; }

    private Result(bool isSuccess, T? value, Error error) : base(isSuccess, error)
        => Value = value;

    public static Result<T> Ok(T value) => new(true, value, Error.None);
    public static new Result<T> Fail(string code, string message) => new(false, default, new Error(code, message));
  }
}
