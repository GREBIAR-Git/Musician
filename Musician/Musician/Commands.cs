namespace Musician
{
    internal class Commands
    {
        public readonly static char prefix = '!';
        public readonly static Command help = new Command("help", "помощь");
        public readonly static Command play = new Command("play", "играть", "проиграть");
        public readonly static Command connect = new Command("connect", "join", "присоединиться", "зайти");
        public readonly static Command disconnect = new Command("disconnect", "leave", "выйти", "покинуть");
        public readonly static Command clear = new Command("clear", "удалить");

        public static bool FindCommand(Command typeСommand, string command)
        {
            string[] elems = command.Split(' ');
            string prefix = elems[0].ToLower();
            if (typeСommand.Contains(prefix))
            {
                command = String.Join(" ", elems, 1, elems.Length - 1);
                return true;
            }
            return false;
        }

        public static string? FindCommands(string command)
        {
            if (FindCommand(help, command))
            {
                return help.MainName();
            }
            else if (FindCommand(play, command))
            {
                return play.MainName();
            }
            else if (FindCommand(disconnect, command))
            {
                return disconnect.MainName();
            }
            else if (FindCommand(connect, command))
            {
                return connect.MainName();
            }
            else if (FindCommand(clear, command))
            {
                return clear.MainName();
            }
            return null;
        }

        public static string Help()
        {
            return OneCommand("help", help.AllCommans()) + "\n" +
                OneCommand("play", play.AllCommans()) + "\n" +
                //OneCommand("stop", stop.AllCommans()) + "\n" +
                OneCommand("connect", connect.AllCommans()) + "\n" +
                OneCommand("disconnect", disconnect.AllCommans()) + "\n" +
                OneCommand("clear", clear.AllCommans());
        }

        static string OneCommand(string name, string[] commands)
        {
            return name + " - " + prefix + String.Join(" " + prefix, commands);
        }

    }
}
