using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PayPal.Scheduler
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Run with console or service
            var asService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IJobFactory, SingletonJobFactory>();
                services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                services.AddSingleton<PayPalSchedulerJob>();
                services.AddSingleton(new JobSchedule(jobType: typeof(PayPalSchedulerJob), cronExpression: "0 0/2 13 * * ?"));
                services.AddHostedService<PayPalScheduler>();
            });

            builder.UseEnvironment(asService ? Environments.Production : Environments.Development);

            if (asService)
            {
                var text = $"Prog" + Environment.NewLine;
                File.WriteAllText(@"C:\Temp\ProgRunAsServiceAsync.txt", text);
                await builder.RunAsServiceAsync();
            }
            else
            {
                var text = $"Prog" + Environment.NewLine;
                File.WriteAllText(@"C:\Temp\ProgErrorRunConsoleAsync.txt", text);
                await builder.RunConsoleAsync();
            }
        }
    }
}
