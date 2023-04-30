using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace MakeSimple.SharedKernel.Cronjob
{
    public static class CronServiceExtentions
    {
        public static void AddCronjob(this IServiceCollection services, string mongoConnectionString, string prefix = "hangfire")
        {
            // Add Hangfire services.
            var mongoUrlBuilder = new MongoUrlBuilder(mongoConnectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy(),
                        BackupPostfix = $"{prefix}.migrationbackup"
                    },
                    Prefix = prefix,
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.Poll,
                    // When server timeout job proccessing change to Enqueued. If the configuration is short, the job may be duplicated across many workers
                    InvisibilityTimeout = System.TimeSpan.FromMinutes(5)
                }));
        }
    }
}
