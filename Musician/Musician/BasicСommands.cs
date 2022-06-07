using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;

namespace Musician
{
    public class BasicСommands : ModuleBase<SocketCommandContext>
    {

        [Command("join", RunMode = RunMode.Async)]
        public async Task Connect()
        {
            IAudioClient? fff = await Connected.Connect(Context.Client, Context.Message);
            AddAudioClient(Context.Message.Channel.Id, fff);

        }

        public void AddAudioClient(ulong idChannel, IAudioClient audioClient)
        {
            foreach (Channel channel in DiscordBot.channels)
            {
                if (channel.Same(idChannel))
                {
                    return;
                }
            }
            DiscordBot.channels.Add(new Channel(idChannel, audioClient));
        }

        public void DelAudioClient(ulong idChannel)
        {
            for (int i = 0; i < DiscordBot.channels.Count; i++)
            {
                if (DiscordBot.channels[i].Same(idChannel))
                {
                    DiscordBot.channels.RemoveAt(i);
                    return;
                }
            }
        }

        public IAudioClient? GetCurrentAudioClient(ulong idChannel)
        {
            IAudioClient? fff = null;
            for (int i = 0; i < DiscordBot.channels.Count; i++)
            {
                if (DiscordBot.channels[i].GetAudioClient(Context.Message.Channel.Id, ref fff))
                {
                    return fff;
                }
            }
            return fff;
        }

        [Command("disconnect", RunMode = RunMode.Async)]
        [Alias("leave", "выйти", "покинуть")]
        public async Task Disconnect()
        {
            await Connected.Disconnect(Context.Client, Context.Message);
            DelAudioClient(Context.Message.Channel.Id);
        }

        [Command("play", RunMode = RunMode.Async)]
        [Alias("играть", "проиграть")]
        public async Task Play([Remainder] string song)
        {
            IAudioClient? fff = GetCurrentAudioClient(Context.Message.Channel.Id);
            if (fff == null)
            {
                fff = await Connected.Connect(Context.Client, Context.Message);
            }
            if (fff != null)
            {
                string filePath = "C:\\Users\\nikit\\Downloads\\" + song;
                using var ffmpeg = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-hide_banner -loglevel panic -i \"{filePath}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true

                });
                if (ffmpeg != null)
                {
                    using var output = ffmpeg.StandardOutput.BaseStream;
                    using var discord = fff.CreatePCMStream(AudioApplication.Mixed);
                    try { await output.CopyToAsync(discord); }
                    finally { await discord.FlushAsync(); }
                }
            }
        }

        [Command("help", RunMode = RunMode.Async)]
        [Alias("помощь")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("", false, Banner.Show("Список команд", Commands.Help()));
        }

        [Command("clear", RunMode = RunMode.Async)]
        [Alias("удалить")]
        public async Task Clear([Remainder] string count)
        {
            if (int.TryParse(count, out int number))
            {
                await DeleteMessages(Context.Message, number);
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false, Banner.Show("Пример команды", "!clear 10"));
            }
        }

        static async Task DeleteMessages(SocketMessage message, int number)
        {
            IEnumerable<IMessage> messages = await message.Channel.GetMessagesAsync(number + 1).FlattenAsync();
            await ((ITextChannel)message.Channel).DeleteMessagesAsync(messages);
        }
    }
}
