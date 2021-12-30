using Discord;
using Discord.WebSocket;

namespace Musician
{
    public class Program
    {
        readonly DiscordSocketClient bot = new DiscordSocketClient();
        readonly string token = "OTI1Nzc2MDg2MDg2ODUyNjQ4.YcyCKw.4t0cwh8l4-bpZHNd6_usFmh7unQ";
        readonly char prefix = '!';

        public static void Main(string[] args) => new Program().Initialization().GetAwaiter().GetResult();
        async Task Initialization()
        {
            bot.MessageReceived += Commands;
            bot.Log += Log;

            await bot.LoginAsync(TokenType.Bot, token);
            await bot.StartAsync();

            Console.ReadLine();
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
                if (message.Content.Length>0 && prefix == message.Content[0])
                {
                    string command = message.Content.Substring(1).ToLower();
                    if(Command.help.Contains(command))
                    {
                        await message.Channel.SendMessageAsync("Список команд");
                        await message.Channel.SendMessageAsync("play - "+ prefix + String.Join(" !", Command.play));
                    }
                    else if (Command.play.Contains(command))
                    {
                        await Bot.Connect(bot, message);
                    }
                    else if (Command.disconnect.Contains(command))
                    {
                        await Bot.Disconnect(bot, message);
                    }
                    else if (Command.connect.Contains(command))
                    {
                        await Bot.Connect(bot, message);
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