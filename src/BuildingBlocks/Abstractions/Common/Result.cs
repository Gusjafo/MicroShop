namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  /// <summary>
  /// Represents the outcome of an operation that does not produce a value.
  /// </summary>
  public class Result
  {
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error associated with the operation when it fails.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">True if the operation was successful.</param>
    /// <param name="error">The error describing the failure.</param>
    protected Result(bool isSuccess, Error error)
    {
      this.IsSuccess = isSuccess;
      this.Error = error;
    }

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Ok()
    {
      return new(true, Error.None);
    }

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result Fail(string code, string message)
    {
      return new(false, new Error(code, message));
    }
  }

  /// <summary>
  /// Represents the outcome of an operation that produces a value.
  /// </summary>
  /// <typeparam name="T">The type of value produced by the operation.</typeparam>
  public sealed class Result<T> : Result
  {
    /// <summary>
    /// Gets the value produced by the operation when it succeeds.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="isSuccess">True if the operation was successful.</param>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error describing the failure.</param>
    private Result(bool isSuccess, T? value, Error error) : base(isSuccess, error)
    {
      this.Value = value;
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> instance containing the specified value.
    /// </summary>
    /// <param name="value">The value produced by the operation.</param>
    /// <returns>A successful result containing <paramref name="value"/>.</returns>
    public static Result<T> Ok(T value)
    {
      return new(true, value, Error.None);
    }

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> instance.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A failed result.</returns>
    public static new Result<T> Fail(string code, string message)
    {
      return new(false, default, new Error(code, message));
    }
  }
}
