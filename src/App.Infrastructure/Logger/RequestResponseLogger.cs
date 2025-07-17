using App.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Logger;

public class RequestResponseLogger(ILogger<RequestResponseLogger> logger) : IRequestResponseLogger {
    public void Log(IRequestResponseLogCreator logCreator) {
        logger.LogError(logCreator.LogString());
    }
}