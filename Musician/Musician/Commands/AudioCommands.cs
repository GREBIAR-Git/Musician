using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using Musician.Audio;
using Musician.Commands.Helper;

namespace Musician.Commands;

public class AudioCommands : InteractionModuleBase<SocketInteractionContext>
{
    const int BLOCKSIZE = 2880;

    async Task Play(Channel channel)
    {
        YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
        if (youtubeInfo is not null)
        {
            youtubeInfo.Stream.Seek(0, SeekOrigin.Begin);
            await FollowupAsync(embed: Banner.Show(youtubeInfo.Info));

            var token = channel.CancellationTokenSource.Token;

            using (var stream = channel.AudioClient.CreatePCMStream(AudioApplication.Mixed))

            using (MemoryStream memoryStream = new())
            {
                CommandResult command = await Cli.Wrap("ffmpeg")
                    .WithArguments("-hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                    .WithStandardInputPipe(PipeSource.FromStream(youtubeInfo.Stream))
                    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                    .ExecuteAsync();
                youtubeInfo.AllStreamPos = memoryStream.Length;
                memoryStream.Seek(youtubeInfo.CurrentStreamPos, SeekOrigin.Begin);
                while (true)
                {
                    youtubeInfo.CurrentStreamPos = memoryStream.Position;
                    if (channel.IsPause)
                    {
                        await memoryStream.FlushAsync();
                        return;
                    }
                    byte[] buffer = new byte[BLOCKSIZE];
                    int byteCount = await memoryStream.ReadAsync(buffer.AsMemory(0, BLOCKSIZE));

                    if (byteCount <= 0) break;

                    try
                    {
                        await stream.WriteAsync(buffer.AsMemory(0, byteCount), token);
                    }
                    catch (Exception exception)
                    {
                        break;
                    }
                }
                await memoryStream.FlushAsync();
                channel.SongIsOver();
                channel.CancellationTokenSource = new CancellationTokenSource();
            }
            if (channel.Queue.Count != 0)
            {
                await Play(channel);
            }
        }
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
            YoutubeExplode.Videos.Video video = channel.Queue.Last().Video;
            await FollowupAsync(embed: Banner.Show("Трек: " + video.Title + "\n" + video.Duration + " - вставлен в очередь"));
        }
    }


    [SlashCommand("play", "Проигрывает композицию по ссылке с youtude")]
    public async Task Play(string queue)
    {
        await DeferAsync();
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                await Play(channel, queue);
            }
            else
            {
                channel = await AudioClient.Connect(guildUser.VoiceChannel);
                await Play(channel, queue);
            }
        }
    }

    [SlashCommand("pause", "Ставит на композицию на паузу")]
    public async Task Pause()
    {
        await DeferAsync();
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                await FollowupAsync(embed: Banner.Show("Композиция остановлена"));
                channel.Pause();
            }
        }
    }


    [SlashCommand("resume", "Возобнавляет воспроизведение композиции")]
    public async Task Resume()
    {
        await DeferAsync();
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                channel.Resume();
                await FollowupAsync(embed: Banner.Show("Композиция продолжена"));
                await Play(channel);
            }
        }
    }

    [SlashCommand("stopall", "Удаляет все композиции из очереди и останавливает текущую")]
    public async Task StopAll()
    {
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                if (channel.Queue.Count > 0)
                {
                    await RespondAsync(embed: Banner.Show("Было пропущено " + channel.Queue.Count + " треков"));
                    channel.AllStop();
                }
                else
                {
                    await RespondAsync(embed: Banner.Show("Итак же ничего не играло :("));
                }
            }
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
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                await FollowupAsync(embed: Banner.Show(channel.GetQueueString()));
            }
            else
            {
                await FollowupAsync(embed: Banner.Show("Очередь пуста"));
            }
        }
    }

    [SlashCommand("nowplaying", "Показывает какая композиция играет в данный момент")]
    public async Task NowPlaying()
    {
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
                if (youtubeInfo is not null)
                {
                    await RespondAsync(embed: Banner.Show(youtubeInfo.Info + " Прошло: " + youtubeInfo.CurrentTime));
                }
                else
                {
                    await RespondAsync(embed: Banner.Show("Сейчас ничего не играет"));
                }
            }
            else
            {
                await RespondAsync(embed: Banner.Show("Сейчас ничего не играет"));
            }
        }
    }

    [SlashCommand("skip", "Пропускает текущую композицию")]
    public async Task Skip()
    {
        await DeferAsync();
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
                if (youtubeInfo is not null)
                {
                    channel.Skip();
                    await FollowupAsync(embed: Banner.Show(youtubeInfo.Info + " - был пропущен"));
                }
                else
                {
                    await FollowupAsync(embed: Banner.Show("Сейчас ничего не играет"));
                }
            }
        }
    }

}
