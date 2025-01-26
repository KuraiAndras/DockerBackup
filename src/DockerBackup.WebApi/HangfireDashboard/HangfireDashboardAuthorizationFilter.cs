using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DockerBackup.WebApi.HangfireDashboard;

public class DefaultAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
