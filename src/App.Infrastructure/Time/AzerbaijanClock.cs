using System.Runtime.InteropServices;
using App.Core.Interfaces;

namespace App.Infrastructure.Time;

public class AzerbaijanClock : IClock {
    public DateTime Now {
        get {
            var tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Azerbaijan Standard Time"
                : "Asia/Baku";

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
    }
}