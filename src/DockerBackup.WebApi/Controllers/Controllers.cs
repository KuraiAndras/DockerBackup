//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using DockerBackup.ApiClient;

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 612 // Disable "CS0612 '...' is obsolete"
#pragma warning disable 649 // Disable "CS0649 Field is never assigned to, and will always have its default value null"
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"
#pragma warning disable 3016 // Disable "CS3016 Arrays as attribute arguments is not CLS-compliant"
#pragma warning disable 8603 // Disable "CS8603 Possible null reference return"
#pragma warning disable 8604 // Disable "CS8604 Possible null reference argument for parameter"
#pragma warning disable 8625 // Disable "CS8625 Cannot convert null literal to non-nullable reference type"
#pragma warning disable 8765 // Disable "CS8765 Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes)."

namespace DockerBackup.WebApi.Controllers
{
    using System = global::System;
    #nullable enable

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public interface IController
    {

        /// <summary>
        /// Get containers
        /// </summary>

        /// <remarks>
        /// Get the list of containers
        /// </remarks>

        /// <returns>OK</returns>

        System.Threading.Tasks.Task<SwaggerResponse<System.Collections.Generic.ICollection<Container>>> GetContainersAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0))")]

    public partial class Controller : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private IController _implementation;

        public Controller(IController implementation)
        {
            _implementation = implementation;
        }

        /// <summary>
        /// Get containers
        /// </summary>
        /// <remarks>
        /// Get the list of containers
        /// </remarks>
        /// <returns>OK</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("api/containers", Name = "GetContainers")]
        public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetContainers(System.Threading.CancellationToken cancellationToken)
        {

            var result = await _implementation.GetContainersAsync(cancellationToken).ConfigureAwait(false);

            var status = result.StatusCode;
            Microsoft.AspNetCore.Mvc.ObjectResult response = new Microsoft.AspNetCore.Mvc.ObjectResult(result.HasProblemDetails ? result.ProblemDetails : result.Result) { StatusCode = status };

            foreach (var header in result.Headers)
                Request.HttpContext.Response.Headers.Add(header.Key, new Microsoft.Extensions.Primitives.StringValues(header.Value.ToArray()));

            return response;
        }

    }

    


    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class SwaggerResponse
    {
        public int StatusCode { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public SwaggerResponse(int statusCode, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers)
        {
            StatusCode = statusCode;
            Headers = headers;
        }

        public static readonly IReadOnlyDictionary<string, IEnumerable<string>> EmptyHeaders = new Dictionary<string, IEnumerable<string>>();

        public static implicit operator SwaggerResponse(int statusCode) => new(statusCode, EmptyHeaders);
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class SwaggerResponse<TResult> : SwaggerResponse where TResult : class
    {
        public TResult? Result { get; private set; }

        public ProblemDetails? ProblemDetails { get; private set; }

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(ProblemDetails))]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(Result))]
        public bool HasProblemDetails => ProblemDetails != null;

        public SwaggerResponse(int statusCode, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, ProblemDetails problemDetails)
            : base(statusCode, headers)
        {
            Result = null;
            ProblemDetails = problemDetails;
        }

        public SwaggerResponse(int statusCode, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result)
            : base(statusCode, headers)
        {
            Result = result;
            ProblemDetails = null;
        }

        public static implicit operator SwaggerResponse<TResult>(ProblemDetails problemDetails) => new(problemDetails.Status, EmptyHeaders, problemDetails);

        public static implicit operator SwaggerResponse<TResult>(TResult result) => new(StatusCodes.Status200OK, EmptyHeaders, result);
    }



}

#pragma warning restore  108
#pragma warning restore  114
#pragma warning restore  472
#pragma warning restore  612
#pragma warning restore 1573
#pragma warning restore 1591
#pragma warning restore 8073
#pragma warning restore 3016
#pragma warning restore 8603
#pragma warning restore 8604
#pragma warning restore 8625