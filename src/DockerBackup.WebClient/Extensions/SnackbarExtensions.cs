using DockerBackup.ApiClient;
using DockerBackup.ApiClient.Infrastructure;

using MudBlazor;

namespace DockerBackup.WebClient.Extensions;

public static class SnackbarExtensions
{
    public static async Task Run(this ISnackbar snackbar, Func<Task> action, string success, string error)
    {
        try
        {
            await action();
            snackbar.Add(success, Severity.Success);
        }
        catch (Exception ex)
        {
            Action handle = ex switch
            {
                ApiException<ProblemDetails> apiException => () => snackbar.Add(apiException.Result.Title ?? error, Severity.Error),
                ApiException apiException => () => snackbar.Add(apiException.Message, Severity.Error),
                _ => () => snackbar.Add(error, Severity.Error)
            };

            handle();
        }
    }
}
