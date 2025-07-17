using App.Core.Entities.Core;

namespace App.Core.Interfaces;

public interface IHomeDataProvider {
    public IReadOnlyDictionary<Home, List<DateOnly>> GetAllHomes();
}