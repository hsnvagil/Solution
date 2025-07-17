using App.Core.Interfaces;
using Newtonsoft.Json;

namespace App.Infrastructure.Logger;

public class FileRequestResponseLogger : IRequestResponseLogger {
    private readonly string _logFilePath;

    public FileRequestResponseLogger() {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
        Directory.CreateDirectory(logDirectory);

        _logFilePath = Path.Combine(logDirectory, "log.txt");
    }

    public void Log(IRequestResponseLogCreator logCreator) {
        var log = logCreator.Log;

        var serializedLog = JsonConvert.SerializeObject(log, Formatting.Indented);
        File.AppendAllText(_logFilePath,
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {serializedLog}{Environment.NewLine}{Environment.NewLine}");
    }
}