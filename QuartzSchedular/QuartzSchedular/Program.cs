using System;

namespace QuartzSchedular
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler scheduleJobs = new Scheduler();
            scheduleJobs.RunJobsAsync();

            Console.ReadLine();
        }
    }
}
