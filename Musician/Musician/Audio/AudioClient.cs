using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace Musician.Audio;

public static class AudioClient
{
    public readonly static Dictionary<ulong, Channel> channels = [];

    public static async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
    {
        if (user.IsBot)
        {

            if (oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                channels.Remove(oldState.VoiceChannel.Id);
            }
        }
    }

    public static async Task<Channel> Connect(IVoiceChannel voiceChannel)
    {
        IAudioClient audioClient = await voiceChannel.ConnectAsync(true, false, false);
        Channel channel = new(audioClient);
        channels.Add(voiceChannel.Id, channel);
        return channel;
    }
}
