using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace PayPal.Scheduler
{
    [DisallowConcurrentExecution]
    public class PayPalSchedulerJob : IJob
    {
        private readonly ILogger<PayPalSchedulerJob> _logger;

        public PayPalSchedulerJob(ILogger<PayPalSchedulerJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var text = $"{DateTime.Now.ToString("yyyy-MM-dd HH: mm: ss")}, NotifyGaft." + Environment.NewLine;
            File.WriteAllText(@"C:\Temp\Service.Write.txt", text);
            _logger.LogInformation("---------- PayPalSchedulerJob Initiated ----------");
            Run().ContinueWith(
            result =>
            {
                _logger.LogInformation("---------- NotifyGaft PayPalJob::End ----------");
            });
            return Task.CompletedTask;
        }

        public async Task Run()
        {
            _logger.LogInformation("---------- NotifyGaft PayPalJob::In Process ----------");
            try
            {
                await NotifyGaft();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error:: {ex.Message}");
            }
        }

        private async Task NotifyGaft()
        {
            var text = $"{DateTime.Now.ToString("yyyy-MM-dd HH: mm: ss")}, NotifyGaft." + Environment.NewLine;
            File.WriteAllText(@"C:\Temp\Service.Write.txt", text);
            Console.WriteLine($"[{nameof(PayPalScheduler)}] has been started.....");
        }
    }
}
