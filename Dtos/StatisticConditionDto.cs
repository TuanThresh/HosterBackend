using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class StatisticConditionDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
}