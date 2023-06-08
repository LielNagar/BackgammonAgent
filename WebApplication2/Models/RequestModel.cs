namespace WebApplication2.Models
{
    public class RequestModel
    {
        private int[] dice;
        private Board board;

        public RequestModel(int[] dice, Board board)
        {
            this.Dice = dice;
            this.Board = board;
        }

        public int[] Dice { get => dice; set => dice = value; }
        public Board Board { get => board; set => board = value; }
    }
}
