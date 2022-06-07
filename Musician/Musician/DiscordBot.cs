using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Musician
{
    internal class DiscordBot
    {
        public static List<Channel> channels = new List<Channel>();

        DiscordSocketClient? bot;
        CommandService? commands;
        IServiceProvider? services;
        readonly string token = "OTI1Nzc2MDg2MDg2ODUyNjQ4.YcyCKw.BpRxZxZ1CIlWD_gXdbYz9vItzYY";

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<DiscordBot>()
                .AddSingleton<BasicСommands>()
                .BuildServiceProvider();
        }

        public async Task Initialization()
        {
            commands = new CommandService();
            bot = new DiscordSocketClient();
            services = ConfigureServices();
            commands.CommandExecuted += CommandExecutedAsync;

            bot.MessageReceived += Commands;
            bot.Log += Log;
            bot.Ready += Ready;
            await bot.LoginAsync(TokenType.Bot, token);
            await bot.StartAsync();
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: services);
            Console.ReadLine();
        }

        async Task<Task> Ready()
        {
            if (bot != null)
            {
                if (bot.CurrentUser.Username != "🎵")
                {
                    // bot.CurrentUser.Username = "🎵";
                }
                await bot.SetGameAsync("!help", null, ActivityType.Watching);
            }
            return Task.CompletedTask;
        }

        Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                Console.WriteLine(result.ErrorReason);
                return;
            }

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync($"<{result.Error}>: {result.ErrorReason}");
        }

        async Task Commands(SocketMessage arg)
        {
            SocketUserMessage? msg = arg as SocketUserMessage;
            if (msg != null && commands != null)
            {
                int pos = 0;

                if (msg.HasCharPrefix(Musician.Commands.prefix, ref pos))
                {
                    SocketCommandContext context = new(bot, msg);
                    IResult result = await commands.ExecuteAsync(context, pos, null);
                    if (!result.IsSuccess)
                    {
                        await msg.Channel.SendMessageAsync("", false, Banner.Show("Напиши !help"));
                    }
                }
            }
        }
    }
}
