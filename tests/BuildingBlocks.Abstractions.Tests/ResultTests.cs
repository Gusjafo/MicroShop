using FluentAssertions;
using MicroShop.BuildingBlocks.Abstractions.Common;
using Xunit;

public class ResultTests
{
  [Fact]
  public void Ok_ShouldCarryValue()
  {
    var r = Result<int>.Ok(42);
    _ = r.IsSuccess.Should().BeTrue();
    _ = r.Value.Should().Be(42);
    _ = r.Error.Should().Be(Error.None);
  }

  [Fact]
  public void Fail_ShouldCarryError()
  {
    var r = Result<int>.Fail("E.TEST", "failed");
    _ = r.IsSuccess.Should().BeFalse();
    _ = r.Error.Code.Should().Be("E.TEST");
  }
}
