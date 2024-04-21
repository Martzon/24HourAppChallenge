using ChallengeApp.Application.Common.Interfaces;

namespace ChallengeApp.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
