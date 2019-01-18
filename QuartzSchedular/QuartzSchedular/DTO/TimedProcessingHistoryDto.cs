using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzSchedular.DTO
{
    class TimedProcessingHistoryDto
    {
        public int TimedProcessingHistoryId { get; set; }
        public int JobId { get; set; }
        public DateTime LastRunDateTime { get; set; }
        public bool Status { get; set; }
    }
}
