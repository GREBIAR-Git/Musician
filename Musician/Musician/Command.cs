namespace Musician
{
    internal class Command
    {
        readonly static char prefix = '!';
        public readonly static string[] help = new string[] { "help", "помощь" };
        public readonly static string[] play = new string[] { "play", "играть", "проиграть"};
        public readonly static string[] stop = new string[] { "stop", "остановить", "стоп" };
        public readonly static string[] connect = new string[] { "connect", "join", "присоединиться", "зайти" };
        public readonly static string[] disconnect = new string[] { "disconnect", "leave","выйти", "покинуть" };

        public static bool IsCommand(string message)
        {
            if(message.Length > 0 && message[0] == prefix)
            {
                return true;
            }
            return false;
        }

        public static bool FindCommand(string[] typeСommand, ref string command)
        {
            string[] elems = command.Split(' ');
            string prefix = elems[0];
            if(typeСommand.Contains(prefix))
            {
                command = String.Join(" ",elems,1,elems.Length-1);
                return true;
            }
            return false;
        }

        public static string Help()
        {
            return OneCommand("help", help) + "\n" +
                OneCommand("play", play) + "\n" +
                OneCommand("connect", connect) + "\n" +
                OneCommand("disconnect", disconnect);
        }

        static string OneCommand(string name, string[] commands)
        {
            return name + " - " + prefix + String.Join(" " + prefix, commands);
        }

    }
}
