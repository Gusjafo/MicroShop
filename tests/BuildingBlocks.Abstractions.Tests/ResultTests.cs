using FluentAssertions;
using MicroShop.BuildingBlocks.Abstractions.Common;
using Xunit;

/// <summary>
/// Contains unit tests that validate the <see cref="Result{T}"/> helper type.
/// </summary>
public class ResultTests
{
  /// <summary>
  /// Ensures a successful result exposes the provided value and no error details.
  /// </summary>
  [Fact]
  public void Ok_ShouldCarryValue()
  {
    var r = Result<int>.Ok(42);
    _ = r.IsSuccess.Should().BeTrue();
    _ = r.Value.Should().Be(42);
    _ = r.Error.Should().Be(Error.None);
  }

  /// <summary>
  /// Ensures a failed result exposes the provided error information.
  /// </summary>
  [Fact]
  public void Fail_ShouldCarryError()
  {
    var r = Result<int>.Fail("E.TEST", "failed");
    _ = r.IsSuccess.Should().BeFalse();
    _ = r.Error.Code.Should().Be("E.TEST");
  }
}
