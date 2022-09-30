/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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