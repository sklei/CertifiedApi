using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiRequestor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertTestController : ControllerBase
{
    private readonly CertifiedHttpClient _certifiedHttpClient;

    public CertTestController(CertifiedHttpClient certifiedHttpClient)
    {
        this._certifiedHttpClient = certifiedHttpClient;
    }

    [HttpGet("withcert")]
    public async Task<IActionResult> WithCert(CancellationToken cancellationToken)
    {
        var resp = await _certifiedHttpClient.GetAsync("https://demosite.local:7246/api/WeatherForecast", cancellationToken);
        
        return Ok(resp);
    }

    [HttpGet("withoutcert")]
    public async Task<IActionResult> WithoutCert(CancellationToken cancellationToken)
    {
        var resp = await _certifiedHttpClient.GetAsync("https://demosite.nocert.local:7246/api/WeatherForecast", cancellationToken);
        
        return Ok(resp);
    }

    [HttpGet("withandwithoutcert")]
    public async Task<IActionResult> WithAndWithoutCert(CancellationToken cancellationToken)
    {
        var resp1 = await _certifiedHttpClient.GetAsync("https://demosite.local:7246/api/WeatherForecast", cancellationToken);
        var resp2 = await _certifiedHttpClient.GetAsync("https://demosite.nocert.local:7246/api/WeatherForecast", cancellationToken);
        
        return Ok(new { resp1, resp2 });
    }
}
