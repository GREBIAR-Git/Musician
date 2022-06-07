using Discord.Audio;

namespace Musician
{
    public class Channel
    {
        ulong id;
        IAudioClient? audioClient;
        public bool Same(ulong id)
        {
            if (this.id == id)
            {
                return true;
            }
            return false;
        }

        public bool GetAudioClient(ulong id, ref IAudioClient? audioClient)
        {
            if (id == this.id)
            {
                audioClient = this.audioClient;
                return true;
            }
            return false;
        }

        public Channel(ulong id, IAudioClient audioClient)
        {
            this.id = id;
            this.audioClient = audioClient;
        }
    }
}
