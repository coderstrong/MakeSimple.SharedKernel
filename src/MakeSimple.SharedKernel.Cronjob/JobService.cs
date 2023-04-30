using Hangfire;
using Microsoft.Extensions.Logging;

namespace Trading.Core.Infrastructure.Cronjob
{
    public abstract class JobService<Job> : IJobService where Job : IJobService
    {
        protected readonly ILogger<Job> _logger;
        private readonly List<CronSetting> _settings;
        protected JobService(ILogger<Job> logger,
            List<CronSetting> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public abstract Task ExecuteAsync(params string[] inputs);

        public Task RegisterAsync(CancellationToken cancellation)
        {
            var settings = _settings.Where(e => e.CronName == typeof(Job).Name).ToList();
            foreach (var item in settings)
            {
                _logger.LogInformation($"Starting job {item.CronName}, ex: {item.CronEx}, StartDate {DateTime.Now}");
                RecurringJob.AddOrUpdate<Job>(item.CronCode, e => e.ExecuteAsync(item.CronCode), item.CronEx);
                _logger.LogInformation($"Started job {item.CronName}, ex: {item.CronEx}, StartedDate {DateTime.Now}");
            }

            return Task.CompletedTask;
        }

        public Task RegisterAsync(string cronCode, string cronEx, CancellationToken cancellation)
        {
            _logger.LogInformation($"Starting job {cronCode}, ex: {cronEx}, StartDate {DateTime.Now}");
            RecurringJob.AddOrUpdate<Job>(cronCode, e => e.ExecuteAsync(cronCode), cronEx);
            _logger.LogInformation($"Started job {cronCode}, ex: {cronEx}, StartedDate {DateTime.Now}");

            return Task.CompletedTask;
        }

        public Task RemoveAsync(CancellationToken cancellation)
        {
            var settings = _settings.Where(e => e.CronName == typeof(Job).Name).ToList();
            foreach (var item in settings)
            {
                _logger.LogInformation($"Stoping job {item.CronName}, ex: {item.CronEx}, Stopingdate {DateTime.Now}");
                RecurringJob.RemoveIfExists(item.CronCode);
                _logger.LogInformation($"Stoping job {item.CronName}, ex: {item.CronEx}, Stopeddate {DateTime.Now}");
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string cronCode, CancellationToken cancellation)
        {
            _logger.LogInformation($"Stoping job {cronCode}, Stopingdate {DateTime.Now}");
            RecurringJob.RemoveIfExists(cronCode);
            _logger.LogInformation($"Stoping job {cronCode}, Stopeddate {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}
