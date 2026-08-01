using EduManagement.Core.Application.Services;
using FluentAssertions;

namespace EduManagement.UnitTests;

public class GpaCalculationServiceTests
{
    private readonly GpaCalculationService _sut = new();

    [Theory]
    [InlineData(90, 80, 85, 84.00)]
    [InlineData(100, 100, 100, 100.00)]
    [InlineData(0, 0, 0, 0.00)]
    [InlineData(60, 70, 90, 81.00)]
    public void Calculate_AllScoresPresent_ReturnsWeightedGpa(
        decimal attendance, decimal midterm, decimal final, decimal expected)
    {
        var result = _sut.Calculate(attendance, midterm, final);

        result.Should().Be(expected);
    }

    [Fact]
    public void Calculate_AttendanceMissing_ReturnsNull()
    {
        var result = _sut.Calculate(null, 80, 85);

        result.Should().BeNull();
    }

    [Fact]
    public void Calculate_MidtermMissing_ReturnsNull()
    {
        var result = _sut.Calculate(90, null, 85);

        result.Should().BeNull();
    }

    [Fact]
    public void Calculate_FinalMissing_ReturnsNull()
    {
        var result = _sut.Calculate(90, 80, null);

        result.Should().BeNull();
    }

    [Fact]
    public void Calculate_AllScoresMissing_ReturnsNull()
    {
        var result = _sut.Calculate(null, null, null);

        result.Should().BeNull();
    }

    [Fact]
    public void Calculate_RoundsToTwoDecimalPlaces_AwayFromZero()
    {
        // attendance*0.10 + midterm*0.30 + final*0.60 = 8.335 + ... crafted to hit .xx5 rounding
        var result = _sut.Calculate(83.35m, 0, 0);

        result.Should().Be(8.34m);
    }
}
