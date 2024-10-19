namespace DockerBackup.WebApi.Controllers;

public partial class SwaggerResponse<TResult> : SwaggerResponse
{
    public static implicit operator SwaggerResponse<TResult>(TResult result) => new(StatusCodes.Status200OK, EmptyHeaders, result);
}


public partial class SwaggerResponse
{
    public static readonly IReadOnlyDictionary<string, IEnumerable<string>> EmptyHeaders = new Dictionary<string, IEnumerable<string>>();

        public static SwaggerResponse<T> Convert<T>(T result) => new(StatusCodes.Status200OK, EmptyHeaders, result);

    public static implicit operator SwaggerResponse(int statusCode) => new(statusCode, EmptyHeaders);
}
