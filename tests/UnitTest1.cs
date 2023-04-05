using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetWeather()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/WeatherForecast");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var weather = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(responseString);

        // Assert
        Assert.NotNull(weather);
        Assert.Equal(5, weather.Count());
    }
}
