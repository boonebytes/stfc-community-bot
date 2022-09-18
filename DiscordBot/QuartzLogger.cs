using Quartz.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DiscordBot;

public class QuartzLogger : ILogProvider
{
    private readonly ILogger _logger;
    
    public QuartzLogger(ILogger logger)
    {
        _logger = logger;
        _logger.LogInformation("QuartzLogger created");
    }
    
    public Logger GetLogger(string name)
    {
        _logger.LogInformation("GetLogger {Name}", name);
        return (level, func, ex, parameters) =>
        {
            _logger.LogInformation("GetLogger Function: {Level}, {Parameters}",
                level.ToString(), parameters.ToString());
            LogLevel logLevel;
            switch (level)
            {
                case Quartz.Logging.LogLevel.Trace:
                    logLevel = LogLevel.Trace;
                    break;
                case Quartz.Logging.LogLevel.Debug:
                    logLevel = LogLevel.Debug;
                    break;
                case Quartz.Logging.LogLevel.Info:
                    logLevel = LogLevel.Information;
                    break;
                case Quartz.Logging.LogLevel.Warn:
                    logLevel = LogLevel.Warning;
                    break;
                case Quartz.Logging.LogLevel.Error:
                    logLevel = LogLevel.Error;
                    break;
                case Quartz.Logging.LogLevel.Fatal:
                    logLevel = LogLevel.Critical;
                    break;
                default:
                    logLevel = LogLevel.Error;
                    break;
            }

            if (func != null)
            {
                if (ex == null)
                {
                    _logger.Log(logLevel, func(), parameters);
                }
                else
                {
                    _logger.Log(logLevel, ex, func(), parameters);
                }
                return true;
            }

            return false;
        };
    }

    public IDisposable OpenNestedContext(string message)
    {
        throw new NotImplementedException();
    }

    public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
    {
        throw new NotImplementedException();
    }
}