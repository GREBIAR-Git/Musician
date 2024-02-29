using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Musician.Services.Helper;

namespace Musician.Services;

public class InteractionHandlingService : IHostedService
{
    readonly DiscordSocketClient _discord;
    readonly InteractionService _interactions;
    readonly IServiceProvider _services;

    public InteractionHandlingService(
        DiscordSocketClient discord,
        InteractionService interactions,
        IServiceProvider services,
        ILogger<InteractionService> logger)
    {
        _discord = discord;
        _interactions = interactions;
        _services = services;

        _interactions.Log += msg => LogHelper.OnLogAsync(logger, msg);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _discord.Ready += () => _interactions.RegisterCommandsGloballyAsync();
        _discord.InteractionCreated += OnInteractionAsync;

        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _interactions.Dispose();
        return Task.CompletedTask;
    }

    async Task OnInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            SocketInteractionContext context = new(_discord, interaction);
            IResult? result = await _interactions.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ToString());
            }
        }
        catch
        {
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync()
                    .ContinueWith(msg => msg.Result.DeleteAsync());
            }
        }
    }
}