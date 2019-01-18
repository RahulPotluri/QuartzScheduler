using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzSchedular
{
    public interface ICalendarHelper
    {
        DateTime GetDateTimeNow();
        string GetDayOfWeek(DayOfWeek dayOfWeek);
        long GetMonthDifference(DateTime startDate, DateTime endDate);
        long GetWeeklyDifference(DateTime startDate, DateTime endDate);
        long GetDayDifference(DateTime startDate, DateTime endDate);
    }
}
