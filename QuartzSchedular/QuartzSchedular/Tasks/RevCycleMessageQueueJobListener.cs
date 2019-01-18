using Quartz;
using QuartzSchedular.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuartzSchedular.Tasks
{
    class RevCycleMessageQueueJobListener : IJobListener
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            string output = "Job was Executed " + context.JobDetail.Key + " at time" + context.FireTimeUtc.ToLocalTime();
            //logger.Info(output);
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.Info("Job Was Vetoed");
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.Info("Job was Executed " + context.JobDetail.Key + " at " + context.FireTimeUtc.ToLocalTime());

            CreateNextJobTrigger(context);

            return Task.CompletedTask;
        }

        private void CreateNextJobTrigger(IJobExecutionContext context)
        {
            try
            {
                IScheduler scheduler = context.Scheduler;

                ITimedProcessing timedProcessing = (ITimedProcessing)context.JobDetail.JobDataMap["TimeProcessingObject"];

                DateTimeOffset nextExecutionOffSet = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                ITrigger trigger = TriggerBuilder.Create()
                                    .StartAt(timedProcessing.GetNextExecutionDateTime())
                                    .Build();

                scheduler.RescheduleJob(context.Trigger.Key, trigger);

                if (context.NextFireTimeUtc.HasValue)
                {
                    logger.Info("Job was Executed " + context.JobDetail.Key + " at " + context.NextFireTimeUtc.Value.ToLocalTime());
                }
            }

            catch (Exception ex)
            {
                logger.Error("Invalid Trigger chaining " + ex.Message);
            }
        }

        public string Name
        {
            get { return "RevCycleMessageQueueJobListener"; }
        }
    }
}
