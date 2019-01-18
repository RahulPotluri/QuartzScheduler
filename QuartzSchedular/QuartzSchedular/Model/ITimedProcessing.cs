using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzSchedular.Model
{
    public interface ITimedProcessing
    {
        int ActionId { get; }
        int FrequencyIntervals { get; }
        FrequencyType FrequencyType { get; }
        int JobId { get; }
        string JobName { get; }
        string Parameter { get; }
        DateTime StartDateTime { get; }

        //DateTime NextExecutionTime { get; }        
        DateTime? LastExecutionDateTime { get; }
        DateTime GetNextExecutionDateTime();

    }
}
