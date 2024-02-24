using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using Musician.Audio;
using Musician.Commands.Helper;

namespace Musician.Commands;

public class AudioCommands : InteractionModuleBase<SocketInteractionContext>
{
    async Task Play(Channel channel)
    {
        YoutubeInfo? youtubeInfo = channel.GetCurrentAudio();
        if (youtubeInfo is not null)
        {
            await FollowupAsync(embed: Banner.Show(youtubeInfo.Info));

            var token = channel.CancellationTokenSource.Token;

            var memoryStream = new MemoryStream();
            await Cli.Wrap("ffmpeg")
                .WithArguments("-hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                .WithStandardInputPipe(PipeSource.FromStream(youtubeInfo.Stream))
                .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                .ExecuteAsync();


            using (var test = channel.AudioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    await test.WriteAsync(memoryStream.ToArray().AsMemory(0, (int)memoryStream.Length), token);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    test.FlushAsync().Wait();
                    channel.SongIsOver();
                    channel.CancellationTokenSource = new CancellationTokenSource();
                }
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

    [SlashCommand("stop", "Ставит на композицию на паузу")]
    public async Task Stop()
    {
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                channel.Stop();
            }
        }
    }


    [SlashCommand("resume", "Возобнавляет воспроизведение композиции")]
    public async Task Resume()
    {
        if (Context.User is IGuildUser guildUser)
        {
            if (AudioClient.channels.TryGetValue(guildUser.VoiceChannel.Id, out Channel channel))
            {
                channel.Resume();
            }
        }
    }

    // 10/10
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

    // 10/10
    [SlashCommand("pop", "Попс")]
    public async Task Pop()
    {
        await Play("https://www.youtube.com/watch?v=NhjpSau984w");
    }


    // 10/10
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

    // 10/10
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
                    await RespondAsync(embed: Banner.Show(youtubeInfo.Info));
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
