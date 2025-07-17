using System.Collections.Concurrent;
using App.Core.Entities.Core;
using App.Core.Interfaces;

namespace App.Infrastructure.Providers;

public class InMemoryHomeDataProvider : IHomeDataProvider {
    private readonly ConcurrentDictionary<Home, List<DateOnly>> _homes;

    public InMemoryHomeDataProvider() {
        _homes = LoadHomes();
    }

    private ConcurrentDictionary<Home, List<DateOnly>> LoadHomes() {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var rawHomes = new List<(Home home, List<DateOnly> slots)> {
            (
                new Home { Id = 123, Name = "Home 1" },
                [
                    new(2025, 07, 15),
                    new(2025, 07, 16),
                    new(2025, 07, 17),
                    new(2025, 07, 18),
                    new(2025, 07, 19),
                    new(2025, 07, 20)
                ]
            ),
            (
                new Home { Id = 456, Name = "Home 2" },
                [
                    new(2025, 07, 18),
                    new(2025, 07, 19)
                ]
            )
        };

        var filtered = rawHomes
            .Select(x => new KeyValuePair<Home, List<DateOnly>>(
                x.home,
                x.slots.Where(date => date >= today).ToList()
            ))
            .Where(x => x.Value.Any())
            .ToList();

        return new ConcurrentDictionary<Home, List<DateOnly>>(filtered);
    }

    public IReadOnlyDictionary<Home, List<DateOnly>> GetAllHomes() {
        return _homes;
    }
}