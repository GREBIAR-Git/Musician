namespace Musician
{
    public class Program
    {
        public static void Main(string[] args) => new DiscordBot().Initialization().GetAwaiter().GetResult();
    }
}