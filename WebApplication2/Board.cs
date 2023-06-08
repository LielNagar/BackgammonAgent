namespace WebApplication2
{
    public class Board
    {
        private string name;

        public Board(string name)
        {
            this.Name = name;
        }

        public Board()
        {
            this.Name = "Liel";
        }

        public string Name { get => name; set => name = value; }
    }
}
