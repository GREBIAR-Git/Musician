using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace Musician
{
    internal class Connected
    {
        public static async Task<IAudioClient?> Connect(DiscordSocketClient bot, SocketUserMessage message)
        {
            if (message is SocketUserMessage userMessage)
            {
                SocketCommandContext context = new(bot, userMessage);
                if (context.User is SocketGuildUser user)
                {
                    if (user != null)
                    {
                        IUser clientUser = await context.Channel.GetUserAsync(context.Client.CurrentUser.Id);
                        if (clientUser is IGuildUser bot1)
                        {
                            IVoiceChannel voice = user.VoiceChannel;
                            if (bot1.VoiceChannel == voice && voice != null)
                            {
                                await userMessage.Channel.SendMessageAsync("", false, Banner.Show("Я же уже зашёл в «" + voice.Name + "»"));
                                return await voice.ConnectAsync(true, false, false);
                            }
                            else
                            {
                                if (voice == null)
                                {
                                    await message.Channel.SendMessageAsync("", false, Banner.Show("Не хочу заходить без тебя =("));
                                }
                                else if (voice.GetUserAsync(bot.CurrentUser.Id) != null)
                                {

                                    await message.Channel.SendMessageAsync("", false, Banner.Show("Захожу в канал «" + voice.Name + "»"));
                                    return await voice.ConnectAsync(true, false, false);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static async Task Disconnect(DiscordSocketClient bot, SocketUserMessage message)
        {
            if (message is SocketUserMessage userMessage)
            {
                SocketCommandContext context = new(bot, userMessage);
                if (context.User is SocketGuildUser user)
                {
                    if (user != null)
                    {
                        IUser clientUser = await context.Channel.GetUserAsync(context.Client.CurrentUser.Id);
                        if (clientUser is IGuildUser bot1)
                        {
                            if (bot1.VoiceChannel == null)
                            {
                                await userMessage.Channel.SendMessageAsync("", false, Banner.Show("Я же ещё не зашёл =("));
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync("", false, Banner.Show("Выхожу из канала «" + bot1.VoiceChannel.Name + "»"));
                                await bot1.VoiceChannel.DisconnectAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}
