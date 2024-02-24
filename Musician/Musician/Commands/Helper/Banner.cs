namespace Musician.Commands.Helper;

internal static class Banner
{
    public static Discord.Embed Show(string title, string description = "", Discord.Color? color = null)
    {
        Discord.EmbedBuilder builder = new()
        {
            Title = title,
            Description = description,
            Color = color
        };
        return builder.Build();
    }
}
