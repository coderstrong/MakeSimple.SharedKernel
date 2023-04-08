using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;

namespace Trading.Core.Infrastructure.Cronjob
{
    public class SkipWhenPreviousJobIsRunningAttribute : JobFilterAttribute, IClientFilter, IApplyStateFilter
    {
        public TimeSpan CheckServerTimeout { get; set; } = new TimeSpan(0, 5, 0);
        private const string RunningKey = "Running";
        private const string JobIdKey = "JobId";

        public void OnCreating(CreatingContext filterContext)
        {
            var connection = filterContext.Connection as JobStorageConnection;

            // We can't handle old storages
            if (connection == null) return;

            // Remove server if heath check timeout 5 minutes
            connection.RemoveTimedOutServers(CheckServerTimeout);

            // We should run this filter only for background jobs based on 
            // recurring ones
            if (!filterContext.Parameters.ContainsKey("RecurringJobId")) return;

            var recurringJobId = filterContext.Parameters["RecurringJobId"] as string;

            // RecurringJobId is malformed. This should not happen, but anyway.
            if (String.IsNullOrWhiteSpace(recurringJobId)) return;

            var running = connection.GetValueFromHash($"recurring-job:{recurringJobId}", RunningKey);
            var jobId = connection.GetValueFromHash($"recurring-job:{recurringJobId}", JobIdKey);
            if ("yes".Equals(running, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(jobId))
            {
                var jobData = connection.GetJobData(jobId);
                if (jobData != null && ProcessingState.StateName.Equals(jobData.State, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[OnCreating] {recurringJobId} is {running}, Canceled!");
                    filterContext.Canceled = true;
                }
            }
        }

        public void OnCreated(CreatedContext filterContext)
        {
            //
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var recurringJobId = SerializationHelper.Deserialize<string>(context.Connection.GetJobParameter(context.BackgroundJob.Id, "RecurringJobId"));
            if (String.IsNullOrWhiteSpace(recurringJobId)) return;

            if (context.NewState is EnqueuedState)
            {
                transaction.SetRangeInHash(
                    $"recurring-job:{recurringJobId}",
                    new[] { new KeyValuePair<string, string>(RunningKey, "yes"), new KeyValuePair<string, string>(JobIdKey, context.BackgroundJob.Id) });

                Console.WriteLine($"[OnStateApplied] EnqueuedState set {recurringJobId} is running YES!");
            }
            else if (context.NewState is SucceededState || context.NewState.IsFinal || context.NewState is FailedState)
            {
                transaction.SetRangeInHash(
                    $"recurring-job:{recurringJobId}",
                    new[] { new KeyValuePair<string, string>(RunningKey, "no"), new KeyValuePair<string, string>(JobIdKey, "") });

                Console.WriteLine($"[OnStateApplied] set {recurringJobId} is running NO!");
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            //
        }
    }
}
