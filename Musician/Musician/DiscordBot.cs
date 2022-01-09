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
                        await message.Channel.SendMessageAsync("",false, Banner("Список команд", Command.Help()));
                    }
                    else if (Command.FindCommand(Command.play, ref command))
                    {
                        IAudioClient audioClient = await Bot.Connect(bot, message);
                        if (audioClient != null)
                        {
                            await message.Channel.SendMessageAsync("", false, Banner("Композиция", command));
                            await bot.SetGameAsync(command, null, ActivityType.Listening);
                        }
                    }
                    else if(Command.FindCommand(Command.stop, ref command))
                    {
                        //Остановить проигрывание
                    }
                    else if (Command.FindCommand(Command.connect, ref command))
                    {
                        await Bot.Connect(bot, message);
                    }
                    else if (Command.FindCommand(Command.disconnect, ref command))
                    {
                        await Bot.Disconnect(bot, message);
                    }
                    else if(Command.FindCommand(Command.clear, ref command))
                    {
                        if(int.TryParse(command,out int number))
                        {
                            await DeleteMessages(message, number);
                            return Task.CompletedTask;
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("", false, Banner("Пример команды","!clear 10"));
                        }
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("",false, Banner("Напиши !help"));
                    }
                    await message.DeleteAsync();
                }
            }
            return Task.CompletedTask;
        }

        async Task DeleteMessages(SocketMessage message, int number)
        {
            IEnumerable<IMessage> messages = await message.Channel.GetMessagesAsync(number + 1).FlattenAsync();
            await((ITextChannel)message.Channel).DeleteMessagesAsync(messages);
        }

        public static Discord.Embed Banner(string title, string description)
        {
            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.Title = title;
            builder.Description = description;
            builder.Color = Discord.Color.Blue;
            return builder.Build();
        }

        public static Discord.Embed Banner(string title)
        {
            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.Title = title;
            builder.Color = Discord.Color.Blue;
            return builder.Build();
        }
    }
}
