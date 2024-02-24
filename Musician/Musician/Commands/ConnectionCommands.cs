using Discord;
using Discord.Interactions;
using Musician.Audio;
using Musician.Commands.Helper;

namespace Musician.Commands;

public class ConnectionCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("join", "Присоединяется к вашему голосовому каналу")]
    public async Task ConnectToVoiceChannel()
    {
        if (Context.User is IGuildUser guildUser)
        {
            IVoiceChannel voiceChannel = guildUser.VoiceChannel;
            if (voiceChannel is not null)
            {
                if (!AudioClient.channels.ContainsKey(voiceChannel.Id))
                {
                    await RespondAsync(embed: Banner.Show("Захожу в канал «" + voiceChannel.Name + "»"));
                    await AudioClient.Connect(voiceChannel);
                }
                else
                {
                    await RespondAsync(embed: Banner.Show("Я уже зашёл и смысла 2 раз заходить, я не вижу"));
                }
            }
            else
            {
                await RespondAsync(embed: Banner.Show("Не хочу заходить без тебя =("));
            }
        }
    }

    [SlashCommand("leave", "Выходит из голосового канала")]
    public async Task Disconnect()
    {
        if (Context.User is IGuildUser guildUser)
        {
            IVoiceChannel voiceChannel = guildUser.VoiceChannel;
            if (voiceChannel is not null)
            {
                if (AudioClient.channels.ContainsKey(voiceChannel.Id))
                {
                    await RespondAsync(embed: Banner.Show("Выхожу из канала «" + voiceChannel.Name + "»"));
                    await voiceChannel.DisconnectAsync();
                    AudioClient.channels.Remove(voiceChannel.Id);
                }
                else
                {
                    await RespondAsync(embed: Banner.Show("Я ещё даже не зашёл в канал «" + voiceChannel.Name + "»"));
                }
            }
            else
            {
                await RespondAsync(embed: Banner.Show("Войдите сначала в канал"));
            }
        }
    }
}
