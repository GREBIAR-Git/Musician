using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Musician.Commands.Helper;

namespace Musician.Commands;

public class BasicCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "Выводит список доступных команд")]
    public async Task Help()
    {
        await RespondAsync(embed: Banner.Show("Список команд",
            "/play - Проигрывает композицию по запросу\n" +
            "/pause - Ставит композицию на паузу\n" +
            "/resume - Возобнавляет воспроизведение композиции\n" +
            "/skip - Пропускает текущую композицию\n" +
            "/stopall - Удаляет все композиции из очереди и останавливает текущую\n" +
            "/queue - Отправляет очередь композиций в виде списка\n" +
            "/nowplaying - Показывает какая композиция играет в данный момент\n" +
            "/pop - Включает сборник группы ГРОБ по названием Попс\n" +
            "/join - Присоединяется к вашему голосовому каналу \n" +
            "/leave - Выходит из голосового канала \n" +
            "/clear N - Удаляет n-ое количестов сообщений \n" +
            "/help - Выводит список доступных команд"));
    }

    [SlashCommand("clear", "Удаляет n-ое количестов сообщений")]
    public async Task Clear([Remainder] string count)
    {
        await RespondAsync(embed: Banner.Show("Очищение"));
        if (int.TryParse(count, out int number))
        {
            await DeleteMessages(Context.Channel, number);
        }
        else
        {
            await Context.Channel.SendMessageAsync("", false, Banner.Show("Пример команды", "/clear 10"));
        }
    }

    static async Task DeleteMessages(ISocketMessageChannel channel, int number)
    {
        IEnumerable<IMessage> messages = await channel.GetMessagesAsync(number + 1).FlattenAsync();
        await ((ITextChannel)channel).DeleteMessagesAsync(messages);
    }
}