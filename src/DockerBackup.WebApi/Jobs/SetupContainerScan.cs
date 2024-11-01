using DockerBackup.WebApi.Options;

using Hangfire;
using Hangfire.Storage;

using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Jobs;

public sealed class SetupContainerScan(IOptions<BackupOptions> options, IRecurringJobManager recurringJob)
{
    public void Setup()
    {
        recurringJob.AddOrUpdate<UpdateContainerConfigurations>(
            UpdateContainerConfigurations.JobName,
            x => x.Handle(CancellationToken.None),
            options.Value.ContainerScanCron);

        if (options.Value.ScanContainersOnStartup)
        {
            recurringJob.Trigger(UpdateContainerConfigurations.JobName);
        }
    }
}
