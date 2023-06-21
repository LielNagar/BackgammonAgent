namespace WebApplication2.Models
{
    public class CheckerMove
    {
        private int from;
        private int to;

        public CheckerMove(int from, int to)
        {
            this.from = from;
            this.to = to;
        }
        public CheckerMove() { }
        public int From { get => from; set => from = value; }
        public int To { get => to; set => to = value; }
    }
}
