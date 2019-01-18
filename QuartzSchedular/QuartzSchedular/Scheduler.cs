using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz.Impl.Matchers;
using System.Collections.Specialized;
using Quartz.Impl;
using Quartz;
using QuartzSchedular.Tasks;
using QuartzSchedular.Services;
using QuartzSchedular.Model;

namespace QuartzSchedular
{
    internal class Scheduler
    {
        public Scheduler()
        {

        }

        internal async Task RunJobsAsync()
        {
            try
            {
                // construct a scheduler factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);

                // get a scheduler
                IScheduler scheduler = await factory.GetScheduler();

                RevCycleMessageQueueJobListener revCycleMessageQueueJobListener = new RevCycleMessageQueueJobListener();
                scheduler.ListenerManager.AddJobListener(revCycleMessageQueueJobListener, GroupMatcher<JobKey>.GroupEquals("RevCycleQueue"));

                await scheduler.Start();

                IReadOnlyDictionary<IJobDetail, ITrigger> dictJobTriggers = GetJobDetailsAndTriggers();

                foreach (var job in dictJobTriggers)
                {
                    await scheduler.ScheduleJob(job.Key, job.Value);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private IReadOnlyDictionary<IJobDetail, ITrigger> GetJobDetailsAndTriggers()
        {
            TimedProcessingService timedProcessingService = new TimedProcessingService();


            IList<ITimedProcessing> listJobsFromDB = timedProcessingService.GetAllTimedProcess();

            var dictJobDetailTrigger = new Dictionary<IJobDetail, ITrigger>();

            foreach (var job in listJobsFromDB)
            {
                string jobKey = job.JobId + "-" + job.JobName;


                IJobDetail jobDetail = JobBuilder.Create<RevCycleQueueJob>()
                                        .WithIdentity(jobKey, "RevCycleQueue")
                                        .UsingJobData(BuildJobDataMap(job))
                                        .Build();

                ITrigger trigger = TriggerBuilder.Create()
                                    .WithIdentity(job.JobName + "-" + job.FrequencyType)
                                    .ForJob(jobKey, "RevCycleQueue")
                                    .StartAt(job.GetNextExecutionDateTime())
                                    .Build();

                if (!dictJobDetailTrigger.ContainsKey(jobDetail))
                {
                    dictJobDetailTrigger.Add(jobDetail, trigger);
                }
            }

            return dictJobDetailTrigger;

        }





        private JobDataMap BuildJobDataMap(ITimedProcessing job)
        {
            JobDataMap jobDataMap = new JobDataMap();
            jobDataMap.Add("ActionId", job.ActionId);
            jobDataMap.Add("TimeProcessingObject", job);
            return jobDataMap;
        }



        #region NotNeeded
        private void SimpleScheduleJob(IScheduler sched)
        {
            IJobDetail jobDetail = JobBuilder.Create<RevCycleQueueJob>()
                                             .WithIdentity("1", "RevCycleQueue")
                                             .UsingJobData("ActionId", "1")
                                             .Build();

            ITrigger trigger = TriggerBuilder.Create()
                                .WithIdentity("1")
                                .ForJob("1")
                                .StartNow()
                                .Build();

            sched.ScheduleJob(jobDetail, trigger);
        }
        private void PrintAllJobsScheduled(IScheduler scheduler)
        {
            var allTriggerKeys = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            foreach (var triggerKey in allTriggerKeys.Result)
            {
                var triggerdetails = scheduler.GetTrigger(triggerKey);
                var Jobdetails = scheduler.GetJobDetail(triggerdetails.Result.JobKey);

                Console.WriteLine("IsCompleted -" + triggerdetails.IsCompleted + " |  TriggerKey  - " + triggerdetails.Result.Key.Name + " Job key -" + triggerdetails.Result.JobKey.Name);
            }
        }
        #endregion

    }
}
