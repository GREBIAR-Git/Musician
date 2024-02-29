using Discord;

namespace Musician.Commands.Helper;

static class Banner
{
    public static Embed Show(string title, string description = "", Color? color = null)
    {
        EmbedBuilder builder = new()
        {
            Title = title,
            Description = description,
            Color = color
        };
        return builder.Build();
    }
}