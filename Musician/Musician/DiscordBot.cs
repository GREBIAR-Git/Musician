using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace Musician
{
    internal class DiscordBot
    {
        readonly DiscordSocketClient bot = new DiscordSocketClient();

        readonly string token = "";

        public async Task Initialization()
        {
            bot.MessageReceived += Commands;
            bot.Log += Log;
            bot.Ready += Ready;
            await bot.LoginAsync(TokenType.Bot, token);
            await bot.StartAsync();
            Console.ReadLine();
        }

        async Task<Task> Ready()
        {
            if (bot.CurrentUser.Username != "🎵")
            {
                //bot.CurrentUser.Username
            }
            await bot.SetGameAsync("!help", null, ActivityType.Watching);
            return Task.CompletedTask;
        }

        Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        async Task<Task> Commands(SocketMessage message)
        {
            if (!message.Author.IsBot)
            {
                if (Command.IsCommand(message.Content))
                {
                    string command = message.Content.Substring(1);
                    if (Command.FindCommand(Command.help, ref command))
                    {
                        await message.Channel.SendMessageAsync("Список команд");
                        await message.Channel.SendMessageAsync(Command.Help());
                    }
                    else if (Command.FindCommand(Command.play, ref command))
                    {

                        IAudioClient audioClient = await Bot.Connect(bot, message);
                        await bot.SetGameAsync(command, null, ActivityType.Listening);
                        if (audioClient != null)
                        {
                            await bot.SetGameAsync("раюотаю", null, ActivityType.Listening);
                        }
                    }
                    else if (Command.FindCommand(Command.connect, ref command))
                    {
                        await Bot.Connect(bot, message);
                    }
                    else if (Command.FindCommand(Command.disconnect, ref command))
                    {
                        await Bot.Disconnect(bot, message);
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Напиши !help");
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
