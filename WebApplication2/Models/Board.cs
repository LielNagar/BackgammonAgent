using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Board
    {
        List<Cell> cells = new List<Cell>(); // Represents 24 cells that cells[0] belongs to agent's home where there is 2 black players that belong to player
        Cell whitePlayerPrison = new Cell();
        Cell blackPlayerPrison = new Cell();
        Cell whitePlayerBank = new Cell();
        Cell blackPlayerBank = new Cell();

        public Board() { }
        public Board(List<Cell> cells)
        {
            this.Cells = cells;
            this.WhitePlayerPrison = new Cell('W', 0, 24);
            this.BlackPlayerPrison = new Cell('B', 0, 25);
            this.WhitePlayerBank = new Cell('W', 0, 26);
            this.BlackPlayerBank = new Cell('B', 0, 27);
        }
        public Board(List<Cell> cells, Cell whitePlayerPrison, Cell whitePlayerBank, Cell blackPlayerPrison, Cell blackPlayerBank)
        {
            this.Cells = cells;
            this.WhitePlayerPrison = whitePlayerPrison;
            this.BlackPlayerPrison = blackPlayerPrison;
            this.WhitePlayerBank = whitePlayerBank;
            this.BlackPlayerBank = blackPlayerBank;
        }

        public Board(Cell whitePlayerPrison, Cell whitePlayerBank, Cell blackPlayerPrison, Cell blackPlayerBank)
        {
            //this.Cells = cells;
            this.WhitePlayerPrison = whitePlayerPrison;
            this.BlackPlayerPrison = blackPlayerPrison;
            this.WhitePlayerBank = whitePlayerBank;
            this.BlackPlayerBank = blackPlayerBank;
        }


        public List<Cell> Cells { get => cells; set => cells = value; }
        public Cell WhitePlayerPrison { get => whitePlayerPrison; set => whitePlayerPrison = value; }
        public Cell BlackPlayerPrison { get => blackPlayerPrison; set => blackPlayerPrison = value; }
        public Cell WhitePlayerBank { get => whitePlayerBank; set => whitePlayerBank = value; }
        public Cell BlackPlayerBank { get => blackPlayerBank; set => blackPlayerBank = value; }

        public bool checkWinner(char color)
        {
            if (color == 'W')
            {
                if (this.WhitePlayerBank.Count == 15) return true;
                return false;
            }
            else
            {
                if (this.BlackPlayerBank.Count == 15) return true;
                return false;
            }

        }
        public bool isAllPlayersInHome()
        {
            if (this.BlackPlayerPrison.Count > 0) return false;
            else for (int i = 5; i <= 0; i--)
            {
                if (this.Cells[i].Color == 'B' && this.Cells[i].Count > 0) return false;
            }
            return true;
        }

        public bool isThereAnyPlayerBeforeDicePosition(int die)
        {
            for (int i = die - 1; i <= 5; i++)
            {
                if (this.Cells[i].Count > 0) return true;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Board other))
            {
                return false;
            }

            foreach (var cell1 in Cells)
            {
                bool foundMatchingCell = false;
                foreach (var cell2 in other.Cells)
                {
                    if (cell1.Equals(cell2))
                    {
                        foundMatchingCell = true;
                        break;
                    }
                }

                if (!foundMatchingCell)
                {
                    return false;
                }
            }
            if (this.WhitePlayerPrison.Equals(other.WhitePlayerPrison) &&
                this.BlackPlayerPrison.Equals(other.BlackPlayerPrison) &&
                this.BlackPlayerBank.Equals(other.BlackPlayerBank) &&
                this.WhitePlayerBank.Equals(other.WhitePlayerBank))
                return true;
            else return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            foreach (Cell cell in Cells)
            {
                hash = hash * 23 + cell.GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            string board = "";

            for (int i = 0; i < Cells.Count / 2 + 1; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (Cells[i].Count == 0)
                    {
                        board += "_";
                    }
                    else
                    {
                        if (Cells[i].Color == 'W') board += "*";
                        else board += '+';
                    }
                }
                board += "\n";
            }
            return board;
        }
    }
}
