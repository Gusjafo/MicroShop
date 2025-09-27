using FluentAssertions;
using MicroShop.BuildingBlocks.Abstractions.Common;
using Xunit;

public class ResultTests
{
  [Fact]
  public void Ok_ShouldCarryValue()
  {
    var r = Result<int>.Ok(42);
    r.IsSuccess.Should().BeTrue();
    r.Value.Should().Be(42);
    r.Error.Should().Be(Error.None);
  }

  [Fact]
  public void Fail_ShouldCarryError()
  {
    var r = Result<int>.Fail("E.TEST", "failed");
    r.IsSuccess.Should().BeFalse();
    r.Error.Code.Should().Be("E.TEST");
  }
}
