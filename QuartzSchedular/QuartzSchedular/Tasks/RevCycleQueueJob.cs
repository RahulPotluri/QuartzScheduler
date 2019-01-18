using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzSchedular.Tasks
{
    public class RevCycleQueueJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int ActionId = dataMap.GetInt("ActionId");

            Console.WriteLine("Job " + key + " is queueing action id" + ActionId);

            await Task.CompletedTask;
        }
    }
}
