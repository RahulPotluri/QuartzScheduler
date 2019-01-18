using QuartzSchedular.DTO;
using QuartzSchedular.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzSchedular.Services
{
    class TimedProcessingService
    {
        public IList<ITimedProcessing> GetAllTimedProcess()
        {
            List<ITimedProcessing> timedProcessings = new List<ITimedProcessing>();

            IList<TimedProcessingDto> sampleDtos = GetSomeSampleJobs();
            IList<TimedProcessingHistoryDto> sampleHistory = GetSomeSampleHistory();

            foreach (var process in sampleDtos)
            {
                TimedProcessing timedProcessing = new TimedProcessing
                {
                    ActionId = process.ActionId,
                    FrequencyIntervals = process.FrequencyIntervals,
                    FrequencyType = process.FrequencyType,
                    JobId = process.JobId,
                    JobName = process.JobName,
                    Parameter = process.Parameter,
                    StartDateTime = process.StartDateTime,

                };

                if (sampleHistory.Any(x => x.JobId == process.JobId))
                {
                    timedProcessing.LastExecutionDateTime = sampleHistory.Where(x => x.JobId == process.JobId).Select(x => x.LastRunDateTime).FirstOrDefault();
                }

                timedProcessings.Add(timedProcessing);
            }
            return timedProcessings;
        }

        private IList<TimedProcessingHistoryDto> GetSomeSampleHistory()
        {
            List<TimedProcessingHistoryDto> timedProcessingHistoryDtos = new List<TimedProcessingHistoryDto>();

            TimedProcessingHistoryDto processingHistoryDto1 = new TimedProcessingHistoryDto()
            {
                TimedProcessingHistoryId = 1,
                JobId = 1,
                LastRunDateTime = DateTime.Now.AddDays(-1),
                Status = true
            };
            timedProcessingHistoryDtos.Add(processingHistoryDto1);

            TimedProcessingHistoryDto processingHistoryDto2 = new TimedProcessingHistoryDto()
            {
                TimedProcessingHistoryId = 2,
                JobId = 4,
                LastRunDateTime = new DateTime(2017, 12, 18, 14, 00, 00),
                Status = true
            };
            timedProcessingHistoryDtos.Add(processingHistoryDto2);

            return timedProcessingHistoryDtos;
        }

        private IList<TimedProcessingDto> GetSomeSampleJobs()
        {
            List<TimedProcessingDto> timedProcessing = new List<TimedProcessingDto>();

            //run job every 2 days at 10:00 AM starting today
            TimedProcessingDto job1 = new TimedProcessingDto()
            {
                ActionId = 1,
                JobId = 1,
                JobName = "Job1",
                //daily
                FrequencyType = FrequencyType.Daily,
                FrequencyIntervals = 2,
                StartDateTime = new DateTime(2018, 12, 18, 10, 00, 00)
            };
            timedProcessing.Add(job1);

            //Run job every week on Monday at 12:00 PM, Monday is figured out from input date
            TimedProcessingDto job2 = new TimedProcessingDto()
            {
                JobId = 2,
                ActionId = 1,
                JobName = "Job2",
                //TODO Weekly
                FrequencyType = FrequencyType.Weekly,
                FrequencyIntervals = 1,
                StartDateTime = new DateTime(2018, 12, 24, 12, 00, 00),
            };
            timedProcessing.Add(job2);

            //Run job every 1 day a week on Tuesday at 1:00 PM
            TimedProcessingDto job3 = new TimedProcessingDto()
            {
                JobId = 3,
                ActionId = 1,
                JobName = "Job3",
                FrequencyType = FrequencyType.Weekly,
                FrequencyIntervals = 1,
                StartDateTime = new DateTime(2018, 12, 18, 13, 00, 00)
            };
            timedProcessing.Add(job3);

            //Run job every month on 18th at 2:00 PM 
            TimedProcessingDto job4 = new TimedProcessingDto()
            {
                JobId = 4,
                ActionId = 2,
                JobName = "Job4",
                FrequencyType = FrequencyType.Monthly,
                FrequencyIntervals = 1,
                StartDateTime = new DateTime(2016, 12, 18, 14, 00, 00)
            };
            timedProcessing.Add(job4);

            //Run job every hour on 18th at 2:00 PM 
            TimedProcessingDto job5 = new TimedProcessingDto()
            {
                JobId = 5,
                ActionId = 2,
                JobName = "Job5",
                FrequencyType = FrequencyType.Hourly,
                FrequencyIntervals = 1,
                StartDateTime = new DateTime(2018, 12, 18, 14, 00, 00)
            };
            timedProcessing.Add(job5);

            //Run job every 2 minute on 30th Second
            TimedProcessingDto job6 = new TimedProcessingDto()
            {
                JobId = 6,
                ActionId = 3,
                JobName = "Job6",
                FrequencyType = FrequencyType.Minute,
                FrequencyIntervals = 2,
                StartDateTime = new DateTime(2018, 12, 20, 13, 00, 30)
            };
            timedProcessing.Add(job6);

            //Monthly at 29 Jan and every month
            TimedProcessingDto job7 = new TimedProcessingDto()
            {
                JobId = 7,
                ActionId = 3,
                JobName = "Job7",
                FrequencyType = FrequencyType.Monthly,
                FrequencyIntervals = 5,
                StartDateTime = new DateTime(2018, 9, 29, 13, 00, 30)
            };
            timedProcessing.Add(job7);

            return timedProcessing;
        }
    }
}
