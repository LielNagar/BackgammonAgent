using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Cell
    {
        char color;
        int count;
        int position;
        double chanceToGetEaten;

        public Cell(char color, int count, int position)
        {
            this.Color = color;
            this.Count = count;
            this.Position = position;
        }

        public Cell() { }
        public char Color { get => color; set => color = value; }
        public int Count { get => count; set => count = value; }
        public int Position { get => position; set => position = value; }
        public double ChanceToGetEaten { get => chanceToGetEaten; set => chanceToGetEaten = value; }

        public double calculateChanceToGetEaten(Board board)
        {
            this.ChanceToGetEaten = 0;
            DiceRollProbability probability = new DiceRollProbability();
            if(board.WhitePlayerPrison.Count == 0)
            {
                for (int i = this.Position; i >= 0; i--)
                {
                    if (board.Cells[i].Count > 0 && board.Cells[i].Color == 'W')
                    {
                        int valueToEat = this.Position - board.Cells[i].Position;
                        this.ChanceToGetEaten += probability.GetSumProbability(valueToEat);
                    }
                }
            }
            else
            {
                this.chanceToGetEaten = probability.GetSumProbability(this.Position+1);
            }
            return this.ChanceToGetEaten;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Cell))
            {
                return false;
            }

            Cell other = (Cell)obj;
            return this.Count == other.Count
                && this.Position == other.Position
                && this.Color == other.Color;
        }
        public override int GetHashCode()
        {
            return Tuple.Create(Count, Position, Color).GetHashCode();
        }
    }
}
