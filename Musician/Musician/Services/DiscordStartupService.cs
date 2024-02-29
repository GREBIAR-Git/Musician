using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Musician.Audio;
using Musician.Services.Helper;

namespace Musician.Services;

public class DiscordStartupService : IHostedService
{
    readonly IConfiguration _config;
    readonly DiscordSocketClient _discord;

    public DiscordStartupService(DiscordSocketClient discord, IConfiguration config,
        ILogger<DiscordSocketClient> logger)
    {
        _discord = discord;
        _config = config;


        _discord.UserVoiceStateUpdated += AudioClient.UserVoiceStateUpdated;

        _discord.Log += msg => LogHelper.OnLogAsync(logger, msg);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discord.LoginAsync(TokenType.Bot, _config["token"]);
        await _discord.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discord.LogoutAsync();
        await _discord.StopAsync();
    }
}