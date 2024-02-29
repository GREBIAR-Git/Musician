using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace Musician.Audio;

public static class AudioClient
{
    public static readonly Dictionary<ulong, Channel> Channels = [];

    public static Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState oldState,
        SocketVoiceState newState)
    {
        if (user.IsBot)
        {
            if (oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                Channels.Remove(oldState.VoiceChannel.Id);
            }
        }

        return Task.CompletedTask;
    }

    public static async Task<Channel> Connect(IVoiceChannel voiceChannel)
    {
        IAudioClient audioClient = await voiceChannel.ConnectAsync(true);
        Channel channel = new(audioClient);
        Channels.Add(voiceChannel.Id, channel);
        return channel;
    }
}