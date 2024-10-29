namespace DockerBackup.WebApi.Extensions;

public static class HostExtensions
{
    public async static Task RunJob<TService>(this IHost host, Func<TService, ValueTask> job) where TService : notnull
    {
        await using var scope = host.Services.CreateAsyncScope();

        var service = scope.ServiceProvider.GetRequiredService<TService>();

        await job(service);
    }

    public async static Task RunJob<TService>(this IHost host, Action<TService> job) where TService : notnull
    {
        await using var scope = host.Services.CreateAsyncScope();

        var service = scope.ServiceProvider.GetRequiredService<TService>();

        job(service);
    }
}
