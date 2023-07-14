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
        private List<Cell> cells = new List<Cell>();
        private Cell whitePlayerPrison = new Cell();
        private Cell blackPlayerPrison = new Cell();
        private Cell whitePlayerBank = new Cell();
        private Cell blackPlayerBank = new Cell();
        private double heuristicScore;
        private int numOfHomeHouses;
        private int userDestination;
        private int agentDestination;
        private int agentCheckersInside;
        private List<CheckerMove> checkerMoves = new List<CheckerMove>();
        private BoardState state;
        private List<Cell> openPlayersPosition = new List<Cell>();

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
        public double HeuristicScore { get => heuristicScore; set => heuristicScore = value; }
        public int NumOfHomeHouses { get => numOfHomeHouses; set => numOfHomeHouses = value; }
        public int UserDestination { get => userDestination; set => userDestination = value; }
        public int AgentDestination { get => agentDestination; set => agentDestination = value; }
        public List<CheckerMove> CheckerMoves { get => checkerMoves; set => checkerMoves = value; }
        public BoardState State { get => state; set => state = value; }
        public int AgentCheckersInside { get => agentCheckersInside; set => agentCheckersInside = value; }

        public int getEatenPlayers()
        {
            return this.WhitePlayerPrison.Count;
        }

        public int getHomeHouses()
        {
            int count = 0;
            for(int i=0; i<=5; i++)
            {
                if (this.Cells[i].Count > 1 && this.Cells[i].Color == 'B') count++;
            }
            return count;
        }

        public List<Cell> getOpenPlayers()
        {
            foreach(Cell cell in this.Cells)
            {
                if (cell.Count == 1 && cell.Color == 'B') this.openPlayersPosition.Add(cell);
            }
            return this.openPlayersPosition;
        }

        public int isAgentInsideUserHome()
        {
            int count = 0;
            for(int i = 23; i>=18; i--)
            {
                if (this.Cells[i].Color == 'B' && this.Cells[i].Count > 0) count++;
            }
            this.AgentCheckersInside = count;
            if (count == 0) return 0;
            return count;
        }

        public BoardState getState()
        {
            int absoluteDiff = Math.Abs(this.AgentDestination - this.UserDestination);
            int count = this.blackPlayerBank.Count;

            for (int i = 0; i <= 5; i++)
            {
                if (this.Cells[i].Count > 0 && this.Cells[i].Color == 'B') count += this.Cells[i].Count;
            }

            if (count == 15) this.state = BoardState.Bearing;

            else if (isAgentInsideUserHome()>0)
                if (this.AgentDestination < 100) this.state = BoardState.NeedToRun;
            else this.state = BoardState.Playing;

            return state;
        }

        public bool isThereAThreat()
        {
            bool isPlayerFound = false;
            if (this.WhitePlayerPrison.Count > 0) return true;
            for(int i=23; i>=0; i--)
            {
                if (this.Cells[i].Color == 'B' && this.Cells[i].Count > 0)
                {
                    isPlayerFound = true;
                }
                if (isPlayerFound)
                {
                    for(int j=i; j>=0; j--)
                    {
                        if (this.Cells[i].Color == 'W' && this.Cells[i].Count > 0) return true;
                    }
                }
            }
            return false;
        }

        public bool isUserIsClosed()
        {
            for(int i = 0; i<=5; i++)
            {
                if (!(this.Cells[i].Count > 1 && this.Cells[i].Color == 'B')) return false;
            }
            return true;
        }
        public void heuristic()
        {
            this.openPlayersPosition = this.getOpenPlayers();
            this.State = this.getState();
            Console.WriteLine($"Board state is :{this.State}");
            double itHurts = 0.0;
            switch (this.state)
            {
                case BoardState.Playing:
                    int sum = 0;
                    if (!isThereAThreat())
                    {
                        if (!isAllPlayersInHome())
                        {
                            sum = getDistance();
                        }
                    }
                    int closing6 = 0;
                    if (isUserInsideOurHome())
                    {
                        if (this.Cells[6].Count > 1 && this.Cells[6].Color == 'B') closing6 = 4;
                    }
                    
                    foreach (Cell cell in this.openPlayersPosition)
                    {
                        cell.ChanceToGetEaten = cell.calculateChanceToGetEaten(this);
                        itHurts += cell.ChanceToGetEaten * (24 - cell.Position);
                    }
                    itHurts = normalizeItHurtsByUserHome(itHurts);
                    this.heuristicScore = this.UserDestination + this.whitePlayerPrison.Count + closing6 +
                        2.5 * this.getHomeHouses() - itHurts + 1.5 * calculateChanceToMakeHomeHouse() - getPlayersInsideUserHome() - sum;

                    if (this.Cells[5].Count < 2 && this.Cells[6].Color == 'B') this.HeuristicScore -= 5;
                        break;

                case BoardState.Bearing:
                    int bonus = 0;
                    itHurts = 0;
                    if (isThereAThreat())
                    {
                        foreach (Cell cell in this.openPlayersPosition)
                        {
                            cell.ChanceToGetEaten = cell.calculateChanceToGetEaten(this);
                            itHurts += cell.ChanceToGetEaten * (24 - cell.Position);
                        }
                        itHurts = normalizeItHurtsByUserHome(itHurts);
                        if (isUserIsClosed()) bonus = 100;
                        this.heuristicScore = 100 + this.blackPlayerBank.Count - itHurts +bonus;
                    }
                    else
                        this.heuristicScore = 100 + this.BlackPlayerBank.Count * 200;
                    break;

                case BoardState.Fight:
                    break;

                case BoardState.NeedToRun:
                    this.HeuristicScore = this.UserDestination - this.AgentCheckersInside + this.getHomeHouses() - this.getDistance() - this.isAgentInsideUserHome();
                    break;

                default:
                    break;
            }
        }

        public double normalizeItHurtsByUserHome(double itHurts)
        {
            int count = 0;
            for (int i = 23; i >= 18; i--)
            {
                if (this.Cells[i].Color == 'W' && this.Cells[i].Count > 1) count++;
            }
            switch (count)
            {
                case 1:
                    itHurts *= 1;
                    break;
                case 2: itHurts *= 1;
                    break;
                case 3: itHurts *= 2;
                    break;
                case 4: itHurts *= 3;
                    break;
                case 5: itHurts *= 4;
                    break;
                case 6: itHurts *= 5;
                    break;
            }
            return itHurts;
        }

        private int getDistance()
        {
            int sumOfDistance = 0;
            for (int i = 6; i <= 23; i++)
            {
                if (this.Cells[i].Count > 0 && this.Cells[i].Color == 'B')
                    sumOfDistance += this.Cells[i].Position * this.Cells[i].Count;
            }
            return sumOfDistance;
        }
        private bool isUserInsideOurHome()
        {
            for(int i=0; i<=5; i++)
            {
                if (this.Cells[i].Color == 'W' && this.Cells[i].Count > 0) return true;
            }
            return false;
        }
        private int getPlayersInsideUserHome()
        {
            int count = 0;
            for(int i=23; i>=18; i--)
            {
                if (this.Cells[i].Count > 0 && this.Cells[i].Color == 'B') count += this.Cells[i].Count; 
            }
            return count;
        }
        private double calculateChanceToMakeHomeHouse()
        {
            DiceRollProbability probability= new DiceRollProbability();
            double chance = 0.0;
            for(int i=0; i<=5; i++)
            {
                if (this.Cells[i].Count < 2) // we want to make home in this cell
                {
                    for(int firstPlayer=i+1; firstPlayer <= 11; firstPlayer++) 
                    {
                        if(firstPlayer >=0 || firstPlayer <= 5) // this player inside home and we dont want to ruin the home
                        {
                            if (this.Cells[firstPlayer].Count != 2)
                            {
                                int firstDie = firstPlayer - i;
                                if(firstDie <= 6)
                                {
                                    for (int secondPlayer = firstPlayer; secondPlayer <= 11; secondPlayer++)
                                    {
                                        int secondDie = secondPlayer - i;
                                        if(secondDie <= 6)
                                        {
                                            if (firstPlayer == secondPlayer)
                                            {
                                                if (this.Cells[firstPlayer].Count > 3) chance += probability.GetRollProbability(firstDie, secondDie);
                                            }
                                            else
                                            {
                                                if (secondPlayer >= 0 || secondPlayer <= 5)
                                                {
                                                    if (this.Cells[secondPlayer].Count != 2)
                                                    {
                                                        chance += probability.GetRollProbability(firstDie, secondDie);
                                                    }
                                                }
                                                else
                                                {
                                                    chance += probability.GetRollProbability(firstDie, secondDie);
                                                }
                                            }
                                        }

                                    }
                                }
                                
                            }
                        }

                    }
                }
            }
            Console.WriteLine($"CHANCE TO GET HOME HOUSE:{chance}");
            return chance;
        }
        public void printChance()
        {
            foreach (Cell cell in this.openPlayersPosition)
            {
                cell.ChanceToGetEaten = cell.calculateChanceToGetEaten(this);
                Console.WriteLine($"player of position {cell.Position}, have {cell.ChanceToGetEaten} chance to be eaten, it hurts {cell.ChanceToGetEaten * (24- cell.Position)}");
            }
        }
        public bool isAllPlayersInHome()
        {
            if (this.BlackPlayerPrison.Count > 0) return false;
            else for (int i = 23; i >= 6; i--)
            {
                if (this.Cells[i].Color == 'B' && this.Cells[i].Count > 0) return false;
            }
            return true;
        }

        public bool isThereAnyPlayerBeforeDicePosition(int homeIndex)
        {
            for (int i = homeIndex+1; i <= 6; i++)
            {
                if (this.Cells[i].Count > 0 && this.Cells[i].Color == 'B') return true;
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
