using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace Musician
{
    internal class Bot
    {
        public static async Task<IAudioClient?> Connect(DiscordSocketClient bot, SocketMessage message)
        {
            if(message is SocketUserMessage userMessage)
            {
                SocketCommandContext context = new SocketCommandContext(bot, userMessage);
                if(context.User is SocketGuildUser user)
                {
                    if (user != null)
                    {
                        IUser clientUser = await context.Channel.GetUserAsync(context.Client.CurrentUser.Id);
                        if (clientUser is IGuildUser bot1)
                        {
                            IVoiceChannel voice = user.VoiceChannel;
                            if (bot1.VoiceChannel != null)
                            {
                                await userMessage.Channel.SendMessageAsync("Я же уже зашёл в - " + voice.Name);
                            }
                            else
                            {
                                if (voice == null)
                                {
                                    await message.Channel.SendMessageAsync("Не хочу заходить без тебя =(");
                                }
                                else if (voice.GetUserAsync(bot.CurrentUser.Id) != null)
                                {
                                    await voice.ConnectAsync(true, false, true);
                                    await message.Channel.SendMessageAsync("Захожу в канал - " + voice.Name);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<Task> Disconnect(DiscordSocketClient bot, SocketMessage message)
        {
            if (message is SocketUserMessage userMessage)
            {
                SocketCommandContext context = new SocketCommandContext(bot, userMessage);
                if (context.User is SocketGuildUser user)
                {
                    if (user != null)
                    {
                        IUser clientUser = await context.Channel.GetUserAsync(context.Client.CurrentUser.Id);
                        if (clientUser is IGuildUser bot1)
                        {
                            if (bot1.VoiceChannel == null)
                            {
                                await userMessage.Channel.SendMessageAsync("Я же ещё не зашёл =(");
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync("Выхожу из канала - " + bot1.VoiceChannel.Name);
                                return bot1.VoiceChannel.DisconnectAsync();
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
