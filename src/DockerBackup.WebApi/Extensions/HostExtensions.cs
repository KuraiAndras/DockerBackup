namespace DockerBackup.WebApi.Extensions;

public static class HostExtensions
{
    public async static Task RunJobAsync<TService>(this IHost host, Func<TService, Task> job) where TService : notnull
    {
        await using var scope = host.Services.CreateAsyncScope();

        var service = scope.ServiceProvider.GetRequiredService<TService>();

        await job(service);
    }
}
