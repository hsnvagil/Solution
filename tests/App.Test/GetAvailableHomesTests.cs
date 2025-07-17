using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;

namespace App.Test;

public class GetAvailableHomesTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>> {
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Should_ReturnHomes_When_DatesAreValid() {
        var startDate = "2025-07-18";
        var endDate = "2025-07-20";

        var response = await _client.GetAsync($"/api/available-homes?startDate={startDate}&endDate={endDate}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);

        json["resultStatus"]?["statusCode"]?.Value<int>().Should().Be(1);
        json["data"]?.HasValues.Should().BeTrue();
        json["errorMessage"]?.Type.Should().Be(JTokenType.Null);
    }

    [Fact]
    public async Task Should_ReturnValidationError_When_StartDateIsAfterEndDate() {
        var startDate = "2025-07-20";
        var endDate = "2025-07-15";

        var response = await _client.GetAsync($"/api/available-homes?startDate={startDate}&endDate={endDate}");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var json = JObject.Parse(await response.Content.ReadAsStringAsync());

        json["resultStatus"]?["statusCode"]?.Value<int>().Should().Be(2);
        json["data"]?.Type.Should().Be(JTokenType.Null);
        json["errorMessage"]
            ?.ToString().Should()
            .Contain("Başlanğıc tarixi bitmə tarixindən kiçik və ya ona bərabər olmalıdır.");
    }
}