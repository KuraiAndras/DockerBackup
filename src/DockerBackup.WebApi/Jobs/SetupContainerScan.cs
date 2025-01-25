using DockerBackup.WebApi.Options;

using Hangfire;

using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Jobs;

public sealed class SetupContainerScan(IOptions<BackupOptions> backupOptions, IRecurringJobManager recurringJob)
{
    public void Setup()
    {
        recurringJob.AddOrUpdate<UpdateContainerConfigurations>(
            UpdateContainerConfigurations.JobName,
            x => x.Handle(CancellationToken.None),
            backupOptions.Value.ContainerScanCron);

        if (backupOptions.Value.ScanContainersOnStartup)
        {
            recurringJob.Trigger(UpdateContainerConfigurations.JobName);
        }
    }
}
