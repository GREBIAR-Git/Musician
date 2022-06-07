namespace Musician
{
    class Command
    {
        public string? MainName()
        {
            if (names.Length > 0)
            {
                return names[0];
            }
            else
            {
                return null;
            }
        }

        public string[] AllCommans()
        {
            return names;
        }

        public bool Contains(string name)
        {
            return names.Contains(name);
        }

        readonly string[] names;

        public Command(params string[] names)
        {
            this.names = names;
        }
    }
}
