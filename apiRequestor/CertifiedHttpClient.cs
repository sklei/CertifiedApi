using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ApiRequestor;

public class CertifiedHttpClient : HttpClient
{
    private readonly HttpClient _httpClient;

    public CertifiedHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public new Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken)
    {
        return _httpClient.GetAsync(requestUri, cancellationToken);
    }

    // public Task<HttpResponseMessage> Post(string requestUri, CancellationToken cancellationToken)
    // {
    //     return _httpClient.po(requestUri, cancellationToken);
    // }

    // public async Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync() =>
    //     await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>(
    //         "repos/dotnet/AspNetCore.Docs/branches");
}
