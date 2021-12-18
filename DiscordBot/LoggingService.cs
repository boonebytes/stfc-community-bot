using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public class LoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(DiscordSocketClient discordSocketClient, CommandService commandService, ILogger<LoggingService> logger)
        {
            discordSocketClient.Log += LogAsync;
            commandService.Log += LogAsync;
            _logger = logger;
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                _logger.LogError($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.", cmdException);
            }
            else
            {
                LogLevel level = LogLevel.Warning;
                switch (message.Severity)
                {
                    case LogSeverity.Critical:
                        level = LogLevel.Critical;
                        break;
                    case LogSeverity.Error:
                        level = LogLevel.Error;
                        break;
                    case LogSeverity.Warning:
                        level = LogLevel.Warning;
                        break;
                    case LogSeverity.Info:
                        level = LogLevel.Information;
                        break;
                    case LogSeverity.Verbose:
                        level = LogLevel.Debug;
                        break;
                    case LogSeverity.Debug:
                        level = LogLevel.Trace;
                        break;
                }
                _logger.Log(level, message.ToString());
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}
