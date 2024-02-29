using YoutubeExplode.Videos;

namespace Musician.Audio;

public class YoutubeInfo(Video video, Stream stream)
{
    public Stream Stream = stream;
    public Video Video = video;

    public long CurrentStreamPos { get; set; }

    public long AllStreamPos { private get; set; }

    public string CurrentTime
    {
        get
        {
            if (AllStreamPos == 0)
            {
                return "0";
            }

            return new TimeSpan(0, 0,
                (int)Math.Floor(CurrentStreamPos / (AllStreamPos / Video.Duration.Value.TotalSeconds))).ToString();
        }
    }

    public string Info => "Трек: " + Video.Title + "\nДлительность: " + Video.Duration;
}