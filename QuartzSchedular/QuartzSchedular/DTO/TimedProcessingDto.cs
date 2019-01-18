using QuartzSchedular.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzSchedular.DTO
{
    class TimedProcessingDto
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public FrequencyType FrequencyType { get; set; }
        public DateTime StartDateTime { get; set; }
        public int FrequencyIntervals { get; set; } = 1;
        public int ActionId { get; set; }
        public string Parameter { get; set; }
    }
}
