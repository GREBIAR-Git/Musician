using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Musician.Services;

var discordSocketConfig = new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.All & ~GatewayIntents.GuildPresences & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites
};

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddYamlFile("_config.yml", false);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton(new DiscordSocketClient(discordSocketConfig));
        services.AddSingleton<InteractionService>();
        services.AddHostedService<InteractionHandlingService>();
        services.AddHostedService<DiscordStartupService>();
    })
    .Build();

await host.RunAsync();