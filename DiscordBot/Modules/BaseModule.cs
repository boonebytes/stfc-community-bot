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
using Discord.Interactions;

namespace DiscordBot.Modules;

public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly ILogger<BaseModule> Logger;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseModule(ILogger<BaseModule> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }
    
    protected async Task ModifyResponseAsync(string content = "", bool ephemeral = false, Embed embed = null)
    {
        await Context.Interaction.ModifyOriginalResponseAsync(properties =>
        {
            if (embed != null) properties.Embed = embed;
            properties.Content = content;
            if (ephemeral)
                properties.Flags = MessageFlags.Ephemeral;
            else
                properties.Flags = MessageFlags.None;
        });
    }
}