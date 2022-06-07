namespace Musician
{
    internal static class Banner
    {
        public static Discord.Embed Show(string title, string description)
        {
            Discord.EmbedBuilder builder = new Discord.EmbedBuilder
            {
                Title = title,
                Description = description,
                Color = Discord.Color.Blue
            };
            return builder.Build();
        }

        public static Discord.Embed Show(string title)
        {
            Discord.EmbedBuilder builder = new();
            builder.Title = title;
            builder.Color = Discord.Color.Blue;
            return builder.Build();
        }
    }
}
