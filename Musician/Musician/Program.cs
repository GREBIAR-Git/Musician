using Discord;
using Discord.WebSocket;

namespace Musician
{
    public class Program
    {
        readonly DiscordSocketClient bot = new DiscordSocketClient();
        readonly string token = "";

        public static void Main(string[] args) => new Program().Initialization().GetAwaiter().GetResult();
        async Task Initialization()
        {
            bot.MessageReceived += Commands;
            bot.Log += Log;
            bot.Ready += Ready;
            await bot.LoginAsync(TokenType.Bot, token);
            await bot.StartAsync();

            Console.ReadLine();
        }

        private Task Ready()
        {
            if(bot.CurrentUser.Username!= "🎵")
            {
            }
            return Task.CompletedTask;
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private async Task<Task> Commands(SocketMessage message)
        {
            if (!message.Author.IsBot)
            {
                if (Command.IsCommand(message.Content))
                {
                    string command = message.Content.Substring(1).ToLower();
                    if (Command.FindCommand(Command.help, command))
                    {
                        await message.Channel.SendMessageAsync("Список команд");
                        await message.Channel.SendMessageAsync(Command.Help());
                    }
                    else if (Command.FindCommand(Command.play, command))
                    {
                        await Bot.Connect(bot, message);
                    }
                    else if (Command.FindCommand(Command.connect, command))
                    {
                        await Bot.Connect(bot, message);
                    }
                    else if (Command.FindCommand(Command.disconnect, command))
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