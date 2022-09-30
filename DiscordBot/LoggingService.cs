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

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot;

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
            _logger.LogError(cmdException,
                "[Command/{MessageSeverity}] {CommandAlias} failed to execute in {ContextChannel}",
                message.Severity, cmdException.Command.Aliases.First(), cmdException.Context.Channel);
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