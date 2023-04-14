using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration;

// TODO: Fix this...
// public class ApiTests : IClassFixture<WebApplicationFactory<Api.Program>>
// {
//     private readonly WebApplicationFactory<Api.Program> _apiFactory;
//     private readonly WebApplicationFactory<ApiRequestor.Program> _apiRequestorFactory;

//     public ApiTests(
//         WebApplicationFactory<Api.Program> apiFactory,
//         WebApplicationFactory<ApiRequestor.Program> apiRequestorFactory)
//     {
//         _apiFactory = apiFactory;
//         _apiRequestorFactory = apiRequestorFactory;
//     }

//     [Fact]
//     public async Task GetWeather()
//     {
//         // Arrange
//         var apiClient = _apiFactory.CreateClient();
//         var apiRequestorClient = _apiRequestorFactory.CreateClient();

//         // Act
//         var response = await apiClient.GetAsync("/api/CertTest/withcert");
//         response.EnsureSuccessStatusCode();
//         var responseString = await response.Content.ReadAsStringAsync();
//         var weather = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(responseString);

//         // Assert
//         Assert.NotNull(weather);
//         Assert.Equal(5, weather.Count());
//     }
// }