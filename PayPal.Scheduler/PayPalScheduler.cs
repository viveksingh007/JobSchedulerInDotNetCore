using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PayPal.Scheduler
{
    public class PayPalScheduler : IHostedService, IDisposable
    {
        private ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        public IScheduler Scheduler { get; set; }

        public PayPalScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<JobSchedule> jobSchedules)
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;
        }
      
        public void Dispose()
        {
            var text = $" Error while running: Dispose NotifyGaft." + Environment.NewLine;
            File.WriteAllText(@"C:\Temp\Service.Write.txt", text);
            Console.WriteLine($"[{nameof(PayPalScheduler)}] has been stopped.....");
        }

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    var text = $"{DateTime.Now.ToString("yyyy-MM-dd HH: mm: ss")}, Testing write." + Environment.NewLine;
        //    File.WriteAllText(@"C:\Temp\Service.Write.txt", text);
        //    Console.WriteLine($"[{nameof(PayPalScheduler)}] has been started.....");
        //    return Task.CompletedTask;
        //}

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }
            await Scheduler.Start(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            File.Delete(@"C:\Temp\Service.Write.txt");
            Console.WriteLine($"[{nameof(PayPalScheduler)}] has been stopped.....");
            Thread.Sleep(1000);
            return Task.CompletedTask;
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            try
            {
                return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }
}
