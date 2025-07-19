using System.Collections.Concurrent;
using System.Text.Json;
using App.Core.Entities.Core;
using App.Core.Interfaces;
using App.Infrastructure.Models;

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
        var rawHomes = JsonSerializer.Deserialize<List<RawHomeJsonModel>>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        var today = DateOnly.FromDateTime(DateTime.Today);

        var homePairs = rawHomes?
            .Select(h => new KeyValuePair<Home, List<DateOnly>>(
                new Home { Id = h.Id, Name = h.Name },
                Enumerable.Range(0, h.DayCount)
                    .Select(i => today.AddDays(h.StartOffset + i))
                    .ToList()
            )) ?? Enumerable.Empty<KeyValuePair<Home, List<DateOnly>>>();

        return new ConcurrentDictionary<Home, List<DateOnly>>(homePairs);
    }

    public IReadOnlyDictionary<Home, List<DateOnly>> GetAllHomes() {
        return _homes;
    }
}