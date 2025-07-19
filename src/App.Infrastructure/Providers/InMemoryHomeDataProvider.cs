using System.Collections.Concurrent;
using App.Core.Entities.Core;
using App.Core.Interfaces;

namespace App.Infrastructure.Providers;

public class InMemoryHomeDataProvider : IHomeDataProvider {
    private readonly ConcurrentDictionary<Home, List<DateOnly>> _homes;
    private readonly IClock _clock;

    public InMemoryHomeDataProvider(IClock clock) {
        _clock = clock;
        _homes = LoadHomes();
    }

    private ConcurrentDictionary<Home, List<DateOnly>> LoadHomes() {
        var today = DateOnly.FromDateTime(_clock.Now);
        var start1 = today.AddDays(-3); 
        var start2 = today;

        var rawHomes = new List<(Home home, List<DateOnly> slots)> {
            (
                new Home { Id = 123, Name = "Home 1" },
                Enumerable.Range(0, 6).Select(offset => start1.AddDays(offset)).ToList()
            ),
            (
                new Home { Id = 456, Name = "Home 2" },
                Enumerable.Range(0, 2).Select(offset => start2.AddDays(offset)).ToList()
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