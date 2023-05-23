using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration;

// TODO: Fix this...
public class ApiTests :
    IClassFixture<WebApplicationFactory<Api.Program>>,
    IClassFixture<WebApplicationFactory<ApiRequestor.Program>>
{
    private readonly WebApplicationFactory<Api.Program> _apiFactory;
    private readonly WebApplicationFactory<ApiRequestor.Program> _apiRequestorFactory;
    private readonly WebApplicationFactoryClientOptions _httpConfig;
    private readonly WebApplicationFactoryClientOptions _httpsConfig;

    public ApiTests(
        WebApplicationFactory<Api.Program> apiFactory,
        WebApplicationFactory<ApiRequestor.Program> apiRequestorFactory)
    {
        _apiFactory = apiFactory;
        _apiRequestorFactory = apiRequestorFactory;

        _httpConfig = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost:5218")
        };

        _httpsConfig = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:7246")
        };
    }

    [Fact]
    public async Task GetWeather()
    {
        // Arrange
        var apiClient = _apiFactory.CreateClient(_httpsConfig);
        var apiRequestorClient = _apiRequestorFactory.CreateClient();

        // Act
        var response = await apiClient.GetAsync("/api/WeatherForecast");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var weather = JsonSerializer.Deserialize<IEnumerable<Api.WeatherForecast>>(responseString);

        // Assert
        Assert.NotNull(weather);
        Assert.Equal(5, weather.Count());
    }

    [Fact]
    public async Task GetWeatherWithCerts()
    {
        // Arrange
        var apiClient = _apiFactory.CreateClient(_httpsConfig);
        // var apiRequestorClient = _apiRequestorFactory.CreateClient();

        // Act
        
        _apiFactory.Server.BaseAddress = new Uri("https://localhost:7246");
        var response = await _apiFactory.Server.SendAsync(context =>
        {
            context.Connection.ClientCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("/Users/steven/DEV/TST/CertifiedApi/Certs/demosite.local.crt");
            context.Request.Method = "GET";
            context.Request.Scheme = "https";
            context.Request.Path = "/api/WeatherForecast/authenticated";
        });
        // response.EnsureSuccessStatusCode();
        // var responseString = await response.Content.ReadAsStringAsync();
        // var weather = JsonSerializer.Deserialize<IEnumerable<Api.WeatherForecast>>(responseString);

        // Assert
        // Assert.NotNull(weather);
        // Assert.Equal(5, weather.Count());

        Assert.Equal(200, response.Response.StatusCode);
    }
}