namespace EduManagement.Core.Application.Services;

public class GpaCalculationService
{
    private const decimal AttendanceWeight = 0.10m;
    private const decimal MidtermWeight = 0.30m;
    private const decimal FinalWeight = 0.60m;

    // Returns null until all three component scores are present — a grade is not final
    // until attendance, midterm, and final are all entered.
    public decimal? Calculate(decimal? attendanceScore, decimal? midtermScore, decimal? finalScore)
    {
        if (attendanceScore is null || midtermScore is null || finalScore is null)
        {
            return null;
        }

        var gpa = (attendanceScore.Value * AttendanceWeight)
            + (midtermScore.Value * MidtermWeight)
            + (finalScore.Value * FinalWeight);

        return Math.Round(gpa, 2, MidpointRounding.AwayFromZero);
    }
}
