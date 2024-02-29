using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Musician.Audio;

public class Channel(IAudioClient audioClient)
{
    public IAudioClient? AudioClient { get; set; } = audioClient;

    public bool IsPause { get; set; }

    public bool IsSkip { get; set; }

    public List<YoutubeInfo> Queue { get; set; } = [];

    public void AllStop()
    {
        Queue.Clear();
        IsSkip = true;
    }

    public void Skip()
    {
        IsSkip = true;
    }

    public void Pause()
    {
        IsPause = true;
    }

    public void Resume()
    {
        IsPause = false;
    }

    public async Task AddAudioInQueue(string request)
    {
        YoutubeClient youtube = new();

        IReadOnlyList<VideoSearchResult> videos = await youtube.Search.GetVideosAsync(request);

        string url = videos[0].Url;

        Video video = await youtube.Videos.GetAsync(url);

        StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);

        IStreamInfo audioStreamInfo = streamManifest.GetAudioStreams()
            .Where(s => s.Container == Container.Mp4)
            .GetWithHighestBitrate();
        Stream stream = await youtube.Videos.Streams.GetAsync(audioStreamInfo);

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
        return Queue.FirstOrDefault();
    }

    public string GetQueueString()
    {
        string queue = string.Empty;
        if (Queue.Count > 0)
        {
            for (int i = 0; i < Queue.Count; i++)
            {
                queue += i + 1 + " «" + Queue[i].Video.Title + "» Время: " + Queue[i].Video.Duration + "\n";
            }
        }
        else
        {
            queue = "Очередь пуста";
        }

        return queue;
    }
}