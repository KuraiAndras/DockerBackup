using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DockerBackup.WebApi.HangfireDashboard;

public class DefaultAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context) =>
        context.GetHttpContext().User.Identity?.IsAuthenticated ?? false;
}
