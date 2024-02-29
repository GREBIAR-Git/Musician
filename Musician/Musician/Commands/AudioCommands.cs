using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using Musician.Audio;
using Musician.Commands.Helper;
using YoutubeExplode.Videos;

namespace Musician.Commands;

public class AudioCommands : InteractionModuleBase<SocketInteractionContext>
{
    const int BlockSize = 2880;

    async Task Play(Channel channel)
    {
        do
        {
            YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
            if (youtubeInfo is not null)
            {
                youtubeInfo.Stream.Seek(0, SeekOrigin.Begin);
                await FollowupAsync(embed: Banner.Show(youtubeInfo.Info));

                await using AudioOutStream? stream = channel.AudioClient.CreatePCMStream(AudioApplication.Mixed);
                using MemoryStream memoryStream = new();
                await Cli.Wrap("ffmpeg")
                    .WithArguments("-hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                    .WithStandardInputPipe(PipeSource.FromStream(youtubeInfo.Stream))
                    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                    .ExecuteAsync();
                youtubeInfo.AllStreamPos = memoryStream.Length;
                memoryStream.Seek(youtubeInfo.CurrentStreamPos, SeekOrigin.Begin);
                while (true)
                {
                    youtubeInfo.CurrentStreamPos = memoryStream.Position;
                    if (channel.IsSkip)
                    {
                        await memoryStream.FlushAsync();
                        channel.IsSkip = false;
                        break;
                    }

                    if (channel.IsPause)
                    {
                        await memoryStream.FlushAsync();
                        return;
                    }

                    byte[] buffer = new byte[BlockSize];
                    int byteCount = await memoryStream.ReadAsync(buffer.AsMemory(0, BlockSize));

                    if (byteCount <= 0)
                    {
                        break;
                    }

                    try
                    {
                        await stream.WriteAsync(buffer.AsMemory(0, byteCount));
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                await memoryStream.FlushAsync();
                channel.SongIsOver();
            }
            else
            {
                return;
            }
        } while (channel.Queue.Count != 0);
    }

    public async Task Play(Channel channel, string queue)
    {
        channel.AddAudioInQueue(queue).Wait();
        if (channel.Queue.Count == 1)
        {
            await Play(channel);
        }
        else
        {
            Video video = channel.Queue.Last().Video;
            await FollowupAsync(
                embed: Banner.Show("Трек: " + video.Title + "\n" + video.Duration + " - вставлен в очередь"));
        }
    }


    [SlashCommand("play", "Проигрывает композицию по запросу")]
    public async Task Play([Summary("queue")] [Autocomplete(typeof(YouTubeAutocompleteHandler))] string queue)
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                await Play(channel, queue);
            }
            else
            {
                channel = await AudioClient.Connect(voiceChannel);
                await Play(channel, queue);
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }

    [SlashCommand("pause", "Ставит на композицию на паузу")]
    public async Task Pause()
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                if (!channel.IsPause)
                {
                    await FollowupAsync(embed: Banner.Show("Композиция остановлена"));
                    channel.Pause();
                }
                else
                {
                    await FollowupAsync(embed: Banner.Show("Композиция ужк остановлена"));
                }
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Бот не в канале"));
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }


    [SlashCommand("resume", "Возобнавляет воспроизведение композиции")]
    public async Task Resume()
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                if (channel.IsPause)
                {
                    channel.Resume();
                    await FollowupAsync(embed: Banner.Show("Композиция продолжена"));
                    await Play(channel);
                }
                else
                {
                    await FollowupAsync(embed: Banner.Show("Композиция итак не остановлена"));
                }
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Бот не в канале"));
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }

    [SlashCommand("stopall", "Удаляет все композиции из очереди и останавливает текущую")]
    public async Task StopAll()
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                if (channel.Queue.Count > 0)
                {
                    await FollowupAsync(embed: Banner.Show("Было пропущено " + channel.Queue.Count + " треков"));
                    channel.AllStop();
                }
                else
                {
                    await FollowupAsync(embed: Banner.Show("Итак же ничего не играло :("));
                }
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Бот не в канале"));
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }

    [SlashCommand("pop", "Попс")]
    public async Task Pop()
    {
        await Play("https://www.youtube.com/watch?v=NhjpSau984w");
    }


    [SlashCommand("queue", "Отправляет очередь композиций в виде списка")]
    public async Task Queue()
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                await FollowupAsync(embed: Banner.Show(channel.GetQueueString()));
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Очередь пуста"));
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }

    [SlashCommand("nowplaying", "Показывает какая композиция играет в данный момент")]
    public async Task NowPlaying()
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
                if (youtubeInfo is not null)
                {
                    string pause = string.Empty;

                    if (channel.IsPause)
                    {
                        pause = "\nПоставлено на паузу";
                    }

                    await FollowupAsync(
                        embed: Banner.Show(youtubeInfo.Info + " Прошло: " + youtubeInfo.CurrentTime + pause));
                }
                else
                {
                    await FollowupAsync(embed: Banner.Show("Сейчас ничего не играет"));
                }
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Сейчас ничего не играет"));
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }

    [SlashCommand("skip", "Пропускает текущую композицию")]
    public async Task Skip()
    {
        await DeferAsync();
        if (OnVoiceChannel(out IVoiceChannel voiceChannel))
        {
            if (AudioClient.Channels.TryGetValue(voiceChannel.Id, out Channel channel))
            {
                YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
                if (youtubeInfo is not null)
                {
                    if (channel.IsPause)
                    {
                        channel.SongIsOver();
                        channel.IsPause = false;
                        await Play(channel);
                    }
                    else
                    {
                        channel.Skip();
                    }

                    await FollowupAsync(embed: Banner.Show(youtubeInfo.Info + " - был пропущен"));
                }
                else
                {
                    await FollowupAsync(embed: Banner.Show("Сейчас ничего не играет"));
                }
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Сейчас ничего не играет"));
            }
        }
        else
        {
            await FollowupAsync(embed: Banner.Show("Вы не в канале"));
        }
    }

    public bool OnVoiceChannel(out IVoiceChannel voiceChannel)
    {
        if (Context.User is IGuildUser { VoiceChannel: not null } guildUser)
        {
            voiceChannel = guildUser.VoiceChannel;
            return true;
        }

        voiceChannel = default!;
        return false;
    }
}