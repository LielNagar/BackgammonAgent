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
