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
            IVoiceChannel? voice = GetVoiceC(bot, message);
            if (voice == null)
            {
                await message.Channel.SendMessageAsync("Не хочу заходить один =(");
                return null;
            }
            await message.Channel.SendMessageAsync("Захожу в канал - " + voice.Name);
            return await voice.ConnectAsync(true, false, true);
        }

        public static async Task<Task?> Disconnect(DiscordSocketClient bot,SocketMessage message)
        {
            IVoiceChannel? voice = GetVoiceD(bot, message);
            if (voice == null)
            {
                await message.Channel.SendMessageAsync("Я же ещё не зашёл =(");
                return null;
            }
            await message.Channel.SendMessageAsync("Выхожу из канала - " + voice.Name);
            return voice.DisconnectAsync();
        }

        static IVoiceChannel? GetVoiceC(DiscordSocketClient bot, SocketMessage arg)
        {
            SocketUserMessage? message = arg as SocketUserMessage;
            SocketCommandContext ctx = new SocketCommandContext(bot, message);
            SocketGuildUser? user = ctx.User as SocketGuildUser;
            if (user != null)
            {
                return user.VoiceChannel;
            }
            return null;
        }

        //нет пока инфы как получить чат бота        
        static IVoiceChannel? GetVoiceD(DiscordSocketClient bot, SocketMessage arg)
        {
            SocketUserMessage? message = arg as SocketUserMessage;
            SocketCommandContext ctx = new SocketCommandContext(bot, message);

            SocketGuildUser? user = ctx.User as SocketGuildUser;
            return null;
        }
    }
}
