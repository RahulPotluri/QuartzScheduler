using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Text;

namespace QuartzSchedular
{
    public class CalendarHelper : ICalendarHelper
    {
        public string GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            string dayOfWeekQuartz = string.Empty;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: dayOfWeekQuartz = "SUN"; break;
                case DayOfWeek.Monday: dayOfWeekQuartz = "MON"; break;
                case DayOfWeek.Tuesday: dayOfWeekQuartz = "TUE"; break;
                case DayOfWeek.Wednesday: dayOfWeekQuartz = "WED"; break;
                case DayOfWeek.Thursday: dayOfWeekQuartz = "THU"; break;
                case DayOfWeek.Friday: dayOfWeekQuartz = "FRI"; break;
                case DayOfWeek.Saturday: dayOfWeekQuartz = "SAT"; break;
            }
            return dayOfWeekQuartz;
        }

        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        public virtual long GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            return Math.Abs(DateAndTime.DateDiff(DateInterval.Month, startDate, endDate));
        }

        public virtual long GetWeeklyDifference(DateTime startDate, DateTime endDate)
        {
            return Math.Abs(DateAndTime.DateDiff(DateInterval.WeekOfYear, startDate, endDate));
        }

        public virtual long GetDayDifference(DateTime startDate, DateTime endDate)
        {
            return Math.Abs(DateAndTime.DateDiff(DateInterval.DayOfYear, startDate, endDate));
        }

    }
}
