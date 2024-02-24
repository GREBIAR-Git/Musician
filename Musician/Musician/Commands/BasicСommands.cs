using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Musician.Commands.Helper;

namespace Musician.Commands;
public class BasicСommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "Выводит список доступных команд")]
    public async Task Help()
    {
        await RespondAsync(embed: Banner.Show("Список команд", "/play\n/"));
    }

    [SlashCommand("clear", "Удаляет n-ое количестов сообщений")]
    public async Task Clear([Remainder] string count)
    {
        await RespondAsync(embed: Banner.Show("test «»"));
        if (int.TryParse(count, out int number))
        {
            await DeleteMessages(Context.Channel, number);
        }
        else
        {
            await Context.Channel.SendMessageAsync("", false, Banner.Show("Пример команды", "!clear 10"));
        }
    }

    static async Task DeleteMessages(ISocketMessageChannel channel, int number)
    {
        IEnumerable<IMessage> messages = await channel.GetMessagesAsync(number + 1).FlattenAsync();
        await ((ITextChannel)channel).DeleteMessagesAsync(messages);
    }
}
