using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Musician.Audio;

public class Channel(IAudioClient audioClient)
{
    public IAudioClient? AudioClient { get; set; } = audioClient;

    public bool IsPause { get; set; }

    public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();


    public void AllStop()
    {
        Queue.Clear();
        CancellationTokenSource.Cancel();
    }

    public void Skip()
    {
        CancellationTokenSource.Cancel();
    }

    public void Pause()
    {
        IsPause = true;
    }

    public void Resume()
    {
        IsPause = false;
    }


    public List<YoutubeInfo> Queue { get; set; } = [];

    public async Task AddAudioInQueue(string url)
    {
        var youtube = new YoutubeClient();

        var video = await youtube.Videos.GetAsync(url);

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);

        var audioStreamInfo = streamManifest.GetAudioStreams()
               .Where(s => s.Container == Container.Mp4)
               .GetWithHighestBitrate();
        var stream = await youtube.Videos.Streams.GetAsync(audioStreamInfo);

        Queue.Add(new YoutubeInfo(video, stream));
    }

    public void SongIsOver()
    {
        if (Queue.Count != 0)
        {
            Queue.RemoveAt(0);
        }
    }


    public YoutubeInfo? GetCurrentAudio()
    {
        return Queue.FirstOrDefault(); ;
    }

    public string GetQueueString()
    {
        string queue = string.Empty;
        if (Queue.Count > 0)
        {
            for (int i = 0; i < Queue.Count; i++)
            {
                queue += (i + 1) + " «" + Queue[i].Video.Title.ToString() + "» Время: " + Queue[i].Video.Duration.ToString() + "\n";
            }
        }
        else
        {
            queue = "Очередь пуста";
        }
        return queue;
    }
}
