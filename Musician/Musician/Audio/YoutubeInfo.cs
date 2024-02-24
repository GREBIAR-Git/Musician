using YoutubeExplode.Videos;

namespace Musician.Audio;

public class YoutubeInfo(Video video, Stream stream)
{
    public Video Video = video;

    public Stream Stream = stream;

    public string Info
    {
        get
        {
            return "Трек: " + Video.Title + "\nДлительность: " + Video.Duration;
        }
    }
}
