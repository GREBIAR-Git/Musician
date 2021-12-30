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
            SocketUserMessage? userMessage = message as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(bot, userMessage);
            SocketGuildUser? user = context.User as SocketGuildUser;
            if (user != null)
            {
                IUser clientUser = await context.Channel.GetUserAsync(context.Client.CurrentUser.Id);
                if (clientUser is IGuildUser bot1)
                {
                    if (bot1.VoiceChannel != null)
                    {
                        await userMessage.Channel.SendMessageAsync("Я же уже зашёл в - " + userMessage.Channel.Name);
                    }
                    else
                    {
                        IVoiceChannel voice = user.VoiceChannel;
                        if (voice == null)
                        {
                            await message.Channel.SendMessageAsync("Не хочу заходить без тебя =(");
                        }
                        else if (voice.GetUserAsync(bot.CurrentUser.Id)!=null)
                        {
                            await message.Channel.SendMessageAsync("Захожу в канал - " + voice.Name);
                            return await voice.ConnectAsync(true, false, true);
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<Task?> Disconnect(DiscordSocketClient bot,SocketMessage message)
        {
            SocketUserMessage? userMessage = message as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(bot, userMessage);
            SocketGuildUser? user = context.User as SocketGuildUser;
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
            return null;
        }
    }
}
