using System.Security.Cryptography.X509Certificates;

namespace apiRequestor;

public class CertifiedHandler : HttpClientHandler
{
    private readonly string[] _certifiedHosts;
    private readonly X509Certificate2 _clientCertificate;

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CertifiedHandler> _logger;

    // TODO: Make this configurable.
    public CertifiedHandler(
        IWebHostEnvironment env,
        ILogger<CertifiedHandler> logger
    )
    {
        _env = env;
        _logger = logger;
        _certifiedHosts = new[] { "localhost", "demosite.local" };
        _clientCertificate = new X509Certificate2(Path.Combine(_env.ContentRootPath, "../certs/demosite.local.pfx"), "welkom-1");

        // TODO: Can we do more checks that likely have to deal with self signed certs etc?
        base.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

        _logger.LogDebug("Initialized CertifiedHandler");
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_certifiedHosts.Contains(request.RequestUri!.Host))
        {
            _logger.LogDebug("Certified URI found: {uri}", request.RequestUri);

            if (!ClientCertificates.Contains(_clientCertificate))
            {
                _logger.LogDebug("Client Certificates wasn't available for URI: {uri} Adding...", request.RequestUri);
                ClientCertificates.Add(_clientCertificate);
            }
        }
        else
        {
            _logger.LogDebug("No certs needed for URI: {uri} Clearing...", request.RequestUri);
            ClientCertificates.Clear();
        }

        _logger.LogDebug("Number of loaded certificates: {count}", ClientCertificates.Count);

        return await base.SendAsync(request, cancellationToken);
    }
}