using System.Text.Json;
using App.Core.Common.LogModels;
using App.Core.Interfaces;

namespace App.Infrastructure.Logger;

public class RequestResponseLogCreator : IRequestResponseLogCreator {
    public RequestResponseLog Log { get; } = new();

    public string LogString() {
        var jsonString = JsonSerializer.Serialize(Log);
        return jsonString;
    }
}