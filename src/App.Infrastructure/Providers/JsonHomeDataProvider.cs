using System.Collections.Concurrent;
using System.Text.Json;
using App.Core.Entities.Core;
using App.Core.Interfaces;

namespace App.Infrastructure.Providers;

public class JsonHomeDataProvider : IHomeDataProvider {
    private readonly ConcurrentDictionary<Home, List<DateOnly>> _homes;

    public JsonHomeDataProvider() {
        _homes = LoadHomesFromJson();
    }

    private ConcurrentDictionary<Home, List<DateOnly>> LoadHomesFromJson() {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "homes.json");

        if (!File.Exists(path))
            return new ConcurrentDictionary<Home, List<DateOnly>>();

        var json = File.ReadAllText(path);
        var homes = JsonSerializer.Deserialize<List<Home>>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var homePairs = homes?
            .Where(h => h.AvailableSlots != null && h.AvailableSlots.Any())
            .Select(h => new KeyValuePair<Home, List<DateOnly>>(
                new Home { Id = h.Id, Name = h.Name },
                h.AvailableSlots
                    .Where(slot => slot >= today)
                    .ToList()
            )) ?? Enumerable.Empty<KeyValuePair<Home, List<DateOnly>>>();

        return new ConcurrentDictionary<Home, List<DateOnly>>(homePairs);
    }

    public IReadOnlyDictionary<Home, List<DateOnly>> GetAllHomes() {
        return _homes;
    }
}