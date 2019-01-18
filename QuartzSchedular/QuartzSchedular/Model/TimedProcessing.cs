using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QuartzSchedular.Model
{
    public class TimedProcessing : ITimedProcessing
    {
        public TimedProcessing()
        {
            _calendarHelper = new CalendarHelper();
        }

        public TimedProcessing(ICalendarHelper calendarHelper)
        {
            _calendarHelper = calendarHelper;
        }

        public int JobId { get; set; }
        public string JobName { get; set; }
        public FrequencyType FrequencyType { get; set; }
        public DateTime StartDateTime { get; set; }
        public int FrequencyIntervals { get; set; } = 1;
        public int ActionId { get; set; }
        public string Parameter { get; set; }


        public DateTime? LastExecutionDateTime { get; set; }
        private ICalendarHelper _calendarHelper;
        private string CronExpression { get; }


        #region Methods
        public DateTime GetNextExecutionDateTime()
        {
            DateTime nextexecutionTime = DateTime.Now;

            DateTime now = _calendarHelper.GetDateTimeNow();

            Thread.Sleep(1000);

            if (!LastExecutionDateTime.HasValue && StartDateTime < now)
            {
                return now;
            }

            switch (FrequencyType)
            {
                case FrequencyType.Minute:
                    nextexecutionTime = GetMinuteExecutionTime(now);
                    break;

                case FrequencyType.Hourly:
                    nextexecutionTime = GetHourlyExecutionDate(now);
                    break;

                case FrequencyType.Daily:
                    nextexecutionTime = GetDailyExecutionTime(now);
                    break;

                case FrequencyType.Weekly:
                    nextexecutionTime = GetWeeklyExecutionTime(now);
                    break;

                case FrequencyType.Monthly:
                    nextexecutionTime = GetMonthlyExecutionDate(now);
                    break;
            }
            return nextexecutionTime;
        }

        private DateTime GetMinuteExecutionTime(DateTime now)
        {
            string _cronString = string.Format("{0} 0/{1} * 1/1 * ? *", StartDateTime.Second, FrequencyIntervals);
            CronExpression cronExpression = new CronExpression(_cronString);

            DateTimeOffset? nextExecutionTime = cronExpression.GetNextValidTimeAfter(now);

            return nextExecutionTime.Value.DateTime.ToLocalTime();
        }

        private DateTime GetHourlyExecutionDate(DateTime now)
        {
            string _cronString = string.Format("{0} {1} 0/{2} 1/1 * ? *", StartDateTime.Second, StartDateTime.Minute, FrequencyIntervals);
            CronExpression cronExpression = new CronExpression(_cronString);

            DateTimeOffset? nextExecutionTime = cronExpression.GetNextValidTimeAfter(now);

            return nextExecutionTime.Value.DateTime.ToLocalTime();
        }

        private DateTime GetWeeklyExecutionTime(DateTime now)
        {
            string dayOfWeek = _calendarHelper.GetDayOfWeek(StartDateTime.DayOfWeek);
            string _cronString = string.Format("{0} {1} {2} ? * {3} *", StartDateTime.Second, StartDateTime.Minute, StartDateTime.Hour, dayOfWeek);

            CronExpression cronExpression = new CronExpression(_cronString);
            DateTimeOffset? nextExecutionDate = cronExpression.GetNextValidTimeAfter(now);

            nextExecutionDate = nextExecutionDate.Value.ToLocalTime();

            if (LastExecutionDateTime.HasValue
                && _calendarHelper.GetWeeklyDifference(nextExecutionDate.Value.DateTime, LastExecutionDateTime.Value) > 1)
            {
                nextExecutionDate = now;
            }

            return nextExecutionDate.Value.DateTime;
        }

        private DateTime GetDailyExecutionTime(DateTime now)
        {
            /* Has Intervals
             1) Get difference between Now and StartDate time
             2) Add frequency intervals to difference in days
             3) Divide Result with intervals and then multiply interval
             4) Add this result to start date Time
             */
            DateTime nextExecutionDate = DateTime.Now;

            if (FrequencyIntervals == 1)
            {
                string cronString = string.Format("{0} {1} {2} ? * * *", StartDateTime.Second, StartDateTime.Minute, StartDateTime.Hour);
                CronExpression cronExpression = new CronExpression(cronString);
                DateTimeOffset? nextValidTime = cronExpression.GetNextValidTimeAfter(now);
                if (nextValidTime.HasValue)
                {
                    nextExecutionDate = nextValidTime.Value.DateTime.ToLocalTime();
                }
            }
            else
            {
                long diffInDays = _calendarHelper.GetDayDifference(now, StartDateTime);
                long diffPlusInterval = diffInDays + FrequencyIntervals;
                long diffPlusIntervalByInterval = diffPlusInterval / FrequencyIntervals;
                long diffPlusIntervalByIntervalPlusInterval = (diffPlusIntervalByInterval) * FrequencyIntervals;

                nextExecutionDate = StartDateTime.AddDays(diffPlusIntervalByIntervalPlusInterval);
            }

            if (LastExecutionDateTime.HasValue
                && _calendarHelper.GetDayDifference(nextExecutionDate, LastExecutionDateTime.Value) > FrequencyIntervals)
            {
                nextExecutionDate = now;
            }

            return nextExecutionDate;
        }

        private DateTime GetMonthlyExecutionDate(DateTime now)
        {
            DateTimeOffset? nextExecutionDate;
            string _cronString = string.Empty;
            CronExpression cronExpression;

            if (StartDateTime.Day == 31)
            {
                _cronString = string.Format("{0} {1} {2} L * ? *", StartDateTime.Second, StartDateTime.Minute, StartDateTime.Hour);
                cronExpression = new CronExpression(_cronString);
                nextExecutionDate = cronExpression.GetNextValidTimeAfter(now);
            }
            else
            {
                _cronString = string.Format("{0} {1} {2} {3} * ? *", StartDateTime.Second, StartDateTime.Minute, StartDateTime.Hour, StartDateTime.Day);
                cronExpression = new CronExpression(_cronString);
                nextExecutionDate = cronExpression.GetNextValidTimeAfter(now);

                if (StartDateTime.Day > 28)
                {
                    _cronString = string.Format("{0} {1} {2} L FEB ? *", StartDateTime.Second, StartDateTime.Minute, StartDateTime.Hour);
                    cronExpression = new CronExpression(_cronString);

                    if (nextExecutionDate > cronExpression.GetNextValidTimeAfter(now))
                    {
                        nextExecutionDate = cronExpression.GetNextValidTimeAfter(now);
                    }
                }
            }

            nextExecutionDate = nextExecutionDate.Value.ToLocalTime();

            //If server was down and an execution was missed
            if (LastExecutionDateTime.HasValue
                && _calendarHelper.GetMonthDifference(nextExecutionDate.Value.DateTime, LastExecutionDateTime.Value) > 1)
            {
                nextExecutionDate = now;
            }

            return nextExecutionDate.Value.DateTime;
        }

        #endregion
    }

    public enum FrequencyType
    {
        Minute,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        EndOfMonth,
    }
}
