namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  /// <summary>
  /// Represents an application level error with a code and message.
  /// </summary>
  /// <param name="Code">The machine readable identifier of the error.</param>
  /// <param name="Message">The human readable description of the error.</param>
  public sealed record Error(string Code, string Message)
  {
    /// <summary>
    /// Gets an <see cref="Error"/> instance representing the absence of an error.
    /// </summary>
    public static Error None => new("", "");

    /// <summary>
    /// Returns a string that represents the current error.
    /// </summary>
    /// <returns>A string containing the error code and message.</returns>
    public override string ToString()
    {
      return $"{this.Code}: {this.Message}";
    }
  }
}
