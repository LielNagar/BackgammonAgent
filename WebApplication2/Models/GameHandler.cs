using System.Drawing;
using System.Globalization;

namespace WebApplication2.Models
{
    public class GameHandler
    {
        private List<Board> possibleBoards = new List<Board>();
        private List<Board> biggerDiceFirst = new List<Board>();
        private List<Board> smallerDiceFirst = new List<Board>();
        
        public GameHandler() { }

        public List<Board> PossibleBoards { get => possibleBoards; set => possibleBoards = value; }
        public List<Board> BiggerDiceFirst { get => biggerDiceFirst; set => biggerDiceFirst = value; }
        public List<Board> SmallerDiceFirst { get => smallerDiceFirst; set => smallerDiceFirst = value; }

        private Board copyBoard(Board board)
        {
            Board newBoard = new Board();
            newBoard.Cells = new List<Cell>();

            foreach (Cell newCell in board.Cells) // Create board to work on
            {
                newBoard.Cells.Add(new Cell { Count = newCell.Count, Color = newCell.Color, Position = newCell.Position });
            }
            newBoard.BlackPlayerPrison = new Cell(board.BlackPlayerPrison.Color, board.BlackPlayerPrison.Count, board.BlackPlayerPrison.Position);
            newBoard.WhitePlayerPrison = new Cell(board.WhitePlayerPrison.Color, board.WhitePlayerPrison.Count, board.WhitePlayerPrison.Position);
            newBoard.WhitePlayerBank = new Cell(board.WhitePlayerBank.Color, board.WhitePlayerBank.Count, board.WhitePlayerBank.Position);
            newBoard.BlackPlayerBank = new Cell(board.BlackPlayerBank.Color, board.BlackPlayerBank.Count, board.BlackPlayerBank.Position);

            return newBoard;
        }
        public List<Board> getPossibleBoards(Board currentBoard, int[] dice)
        {
            if (dice[1] == dice[0])
            {
                playDouble(currentBoard, dice[1]);
                return PossibleBoards;
            }
            else
            {
                playBiggerDieFirst(currentBoard, dice[0]);
                playSmallerDieFirst(currentBoard, dice[1]);
            }

            foreach (Board boardAfterBigMove in BiggerDiceFirst)
            {
                playSecondDice(boardAfterBigMove, dice[1]);
            }
            foreach(Board boardAfterSmallMove in SmallerDiceFirst)
            {
                playSecondDice(boardAfterSmallMove, dice[0]);
            }

            //formatPossibleBoards();
            return PossibleBoards;
        }

        private void handlePrisonMove(Board board, int die, string whichDieMove)
        {
            int destination = 24 - die;
            if (board.Cells[destination].Count < 2 || board.Cells[destination].Color == 'B')
            {
                if (board.Cells[destination].Color == 'W') // It means that we can eat the user.
                {
                    board.Cells[destination].Color = 'B';
                    board.Cells[destination].Count = 1;
                    board.BlackPlayerPrison.Count--;
                    board.WhitePlayerPrison.Count++;
                    board.UserDestination += board.Cells[destination].Position + 1;
                }
                else
                {
                    board.BlackPlayerPrison.Count--;
                    board.Cells[destination].Count++;
                    board.Cells[destination].Color = 'B';
                }
            }
            else return;
            switch (whichDieMove)
            {
                case "smallerDie":
                    SmallerDiceFirst.Add(board);
                    return;
                case "biggerDie":
                    BiggerDiceFirst.Add(board);
                    return;
                case "lastDie":
                    if(!PossibleBoards.Contains(board)) PossibleBoards.Add(board);
                    return;
            }
        }

        private void handleAllPlayersInHome(Board board, int die, string whichDieMove)
        {
            int destination;
            for (int k = 5; k >= 0; k--)
            {
                Board homeBoard = copyBoard(board);
                destination = k - die;
                try
                {
                    if (homeBoard.Cells[k].Color == 'B' && homeBoard.Cells[k].Count > 0) // Cell is ours and there is a player
                    {
                        if (k + 1 < die) // Cell that is smaller than the dice
                        {
                            if (!homeBoard.isThereAnyPlayerBeforeDicePosition(die)) // We take out
                            {
                                homeBoard.Cells[k].Count--;
                                if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                homeBoard.BlackPlayerBank.Count++;
                                switch (whichDieMove)
                                {
                                    case "smallerDie":
                                        SmallerDiceFirst.Add(homeBoard); break;
                                    case "biggerDie":
                                        BiggerDiceFirst.Add(homeBoard); break;
                                    case "lastDie":
                                        if (!PossibleBoards.Contains(homeBoard)) PossibleBoards.Add(homeBoard); break;
                                }
                            }
                        }
                        else // We are folding, or we can take out.
                        {
                            if (homeBoard.Cells[destination].Color != 'W') // We can move to this cell and its ours
                            {
                                homeBoard.Cells[k].Count--;
                                homeBoard.Cells[destination].Count++;
                                homeBoard.Cells[destination].Color = 'B';
                                if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                switch (whichDieMove)
                                {
                                    case "smallerDie":
                                        SmallerDiceFirst.Add(homeBoard); break;
                                    case "biggerDie":
                                        BiggerDiceFirst.Add(homeBoard); break;
                                    case "lastDie":
                                        if (!PossibleBoards.Contains(homeBoard)) PossibleBoards.Add(homeBoard); break;
                                }
                            }
                            else if (homeBoard.Cells[destination].Count < 2) // We ate the opponent
                            {
                                homeBoard.Cells[k].Count--;
                                homeBoard.Cells[destination].Count = 1;
                                homeBoard.Cells[destination].Color = 'B';
                                homeBoard.WhitePlayerPrison.Count++;
                                homeBoard.UserDestination += homeBoard.Cells[destination].Position + 1;
                                if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                switch (whichDieMove)
                                {
                                    case "smallerDie":
                                        SmallerDiceFirst.Add(homeBoard); break;
                                    case "biggerDie":
                                        BiggerDiceFirst.Add(homeBoard); break;
                                    case "lastDie":
                                        if (!PossibleBoards.Contains(homeBoard)) PossibleBoards.Add(homeBoard); break;
                                }
                            }
                        }
                    }
                }
                catch (ArgumentOutOfRangeException e) // This is when we are try to take out a player
                {
                    homeBoard.Cells[k].Count--;
                    if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                    homeBoard.BlackPlayerBank.Count++;
                    switch (whichDieMove)
                    {
                        case "smallerDie":
                            SmallerDiceFirst.Add(homeBoard); break;
                        case "biggerDie":
                            BiggerDiceFirst.Add(homeBoard); break;
                        case "lastDie":
                            if (!PossibleBoards.Contains(homeBoard)) PossibleBoards.Add(homeBoard); break;
                    }
                }
            }
        }

        private void handleRegularMove(Board board, int die, string whichDieMove)
        {
            foreach (Cell cell in board.Cells)
            {
                try
                {
                    if (cell.Color == 'B') // Cell is ours
                    {
                        int destination = cell.Position - die;
                        Board newBoard2 = copyBoard(board);
                        if (newBoard2.Cells[destination].Count < 2 || newBoard2.Cells[destination].Color == 'B')
                        {
                            if (newBoard2.Cells[destination].Color == 'W') // We ate the opponent
                            {
                                newBoard2.Cells[destination].Count = 1;
                                newBoard2.WhitePlayerPrison.Count++;
                                newBoard2.UserDestination += newBoard2.Cells[destination].Position + 1;
                                newBoard2.Cells[destination].Color = 'B';
                                newBoard2.Cells[cell.Position].Count--;
                                if (newBoard2.Cells[cell.Position].Count == 0) newBoard2.Cells[cell.Position].Color = '0';
                                switch (whichDieMove)
                                {
                                    case "smallerDie":
                                        SmallerDiceFirst.Add(newBoard2); break;
                                    case "biggerDie":
                                        BiggerDiceFirst.Add(newBoard2); break;
                                    case "lastDie":
                                        if (!PossibleBoards.Contains(newBoard2)) PossibleBoards.Add(newBoard2); break;
                                }
                            }
                            else
                            {
                                newBoard2.Cells[destination].Count++;
                                newBoard2.Cells[destination].Color = 'B';
                                newBoard2.Cells[cell.Position].Count--;
                                if (newBoard2.Cells[cell.Position].Count == 0) newBoard2.Cells[cell.Position].Color = '0';
                                switch (whichDieMove)
                                {
                                    case "smallerDie":
                                        SmallerDiceFirst.Add(newBoard2); break;
                                    case "biggerDie":
                                        BiggerDiceFirst.Add(newBoard2); break;
                                    case "lastDie":
                                        if (!PossibleBoards.Contains(newBoard2)) PossibleBoards.Add(newBoard2); break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e) { }
            }
        }

        private void playDouble(Board board, int die)
        {
            List<Board> oneMoveBoards = new List<Board>() { board };
            List<Board> makedBoards = new List<Board>() { board };
            for(int i=0; i<5; i++)
            {
                if (makedBoards.Count != 0) oneMoveBoards.Clear();
                foreach (Board needToCopy in makedBoards) oneMoveBoards.Add(needToCopy);
                makedBoards.Clear();
                foreach(Board board1 in oneMoveBoards)
                {
                    if (board1.BlackPlayerPrison.Count > 0)
                    {
                        Board newBoard = copyBoard(board1);
                        handlePrisonMove(newBoard, die, "double");
                        makedBoards.Add(newBoard);
                        break;
                    }
                    if (board1.isAllPlayersInHome())
                    {
                        for (int k = 5; k >= 0; k--) // Iterate each cell in home in double one move
                        {
                            int destination = k - die;
                            Board homeBoard = copyBoard(board1);
                            try
                            {
                                if (homeBoard.Cells[k].Color == 'B' && homeBoard.Cells[k].Count > 0) // Cell is ours and there is a player
                                {
                                    if (k + 1 < die) // Cell is smaller than dice
                                    {
                                        if (!homeBoard.isThereAnyPlayerBeforeDicePosition(die)) // We take out
                                        {
                                            homeBoard.Cells[k].Count--;
                                            if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                            homeBoard.BlackPlayerBank.Count++;
                                            makedBoards.Add(homeBoard);
                                        }
                                    }
                                    else
                                    {
                                        if (homeBoard.Cells[destination].Color != 'W') // We can move to this cell (Folding)
                                        {
                                            homeBoard.Cells[k].Count--;
                                            homeBoard.Cells[destination].Count++;
                                            if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                            makedBoards.Add(homeBoard);
                                        }
                                        else // We eat the opponent
                                        {
                                            homeBoard.Cells[k].Count--;
                                            homeBoard.Cells[destination].Count = 1;
                                            homeBoard.WhitePlayerPrison.Count++;
                                            homeBoard.UserDestination += homeBoard.Cells[destination].Position + 1;
                                            if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                            makedBoards.Add(homeBoard);
                                        }
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException e) // We take out a player
                            {
                                homeBoard.Cells[k].Count--;
                                if (homeBoard.Cells[k].Count == 0) homeBoard.Cells[k].Color = '0';
                                homeBoard.BlackPlayerBank.Count++;
                                makedBoards.Add(homeBoard);
                            }
                        }
                        continue;
                    }
                    foreach (Cell cell in board1.Cells) // Play one move for each cell
                    {
                        if (cell.Color == 'B') // Cell is ours
                        {
                            try
                            {
                                int destination = cell.Position - die;
                                Board newBoard = copyBoard(board1);
                                if (newBoard.Cells[destination].Count < 2 ||
                                newBoard.Cells[destination].Color == 'B') // We can play in this cell
                                {
                                    if (newBoard.Cells[destination].Color == 'W') // We ate the opponent
                                    {
                                        newBoard.Cells[destination].Count = 1;
                                        newBoard.Cells[destination].Color = 'B';
                                        newBoard.WhitePlayerPrison.Count++;
                                        newBoard.UserDestination += newBoard.Cells[destination].Position + 1;
                                        newBoard.Cells[cell.Position].Count--;
                                        if (newBoard.Cells[cell.Position].Count == 0) newBoard.Cells[cell.Position].Color = '0';
                                        makedBoards.Add(newBoard);
                                    }
                                    else
                                    {
                                        newBoard.Cells[destination].Count++;
                                        newBoard.Cells[destination].Color = 'B';
                                        newBoard.Cells[cell.Position].Count--;
                                        if (newBoard.Cells[cell.Position].Count == 0) newBoard.Cells[cell.Position].Color = '0';
                                        makedBoards.Add(newBoard);
                                    }
                                }
                            }
                            catch (Exception e) { continue; }
                        }
                    }
                }
            }
            foreach (Board board2 in oneMoveBoards) // Copy the final results after 4 moves into possibleBoards.
            {
                if(!PossibleBoards.Contains(board2)) PossibleBoards.Add(board2);
            }
            //formatPossibleBoards();
        }

        private void playSmallerDieFirst(Board board, int die)
        {
            Board newBoard = copyBoard(board);
            if (newBoard.BlackPlayerPrison.Count > 0)
            {
               handlePrisonMove(newBoard, die, "smallerDie");
                return;
            }
            if (newBoard.isAllPlayersInHome())
            {
                handleAllPlayersInHome(newBoard, die, "smallerDie");
                return;
            }
            handleRegularMove(board, die, "smallerDie");
        }

        private void playBiggerDieFirst(Board board, int die)
        {
            Board newBoard = copyBoard(board);
            if(newBoard.BlackPlayerPrison.Count > 0)
            {
               handlePrisonMove(newBoard, die, "biggerDie");
               return;
            }
            if (newBoard.isAllPlayersInHome())
            {
                handleAllPlayersInHome(newBoard, die, "biggerDie");
                return;
            }
            handleRegularMove(board, die, "biggerDie");
        }

        private void playSecondDice(Board board, int die)
        {
            Board newBoard = copyBoard(board);
            if (newBoard.BlackPlayerPrison.Count > 0)
            {
                handlePrisonMove(newBoard, die, "lastDie");
                return;
            }
            if (newBoard.isAllPlayersInHome())
            {
                handleAllPlayersInHome(newBoard, die, "lastDie");
                return;
            }
            handleRegularMove(board, die, "lastDie");
        }

        public void formatPossibleBoards()
        {
            if (PossibleBoards.Count == 0)
            {
                if (BiggerDiceFirst.Count == 0 && SmallerDiceFirst.Count > 0) PossibleBoards = SmallerDiceFirst;
                if (BiggerDiceFirst.Count > 0 && SmallerDiceFirst.Count == 0) PossibleBoards = BiggerDiceFirst;
            }
            else
            {
                Console.WriteLine($"There are {PossibleBoards.Count} possible boards");
                List<Board> unique = new List<Board>();
                foreach (Board board in PossibleBoards)
                {
                    if (!unique.Contains(board))
                    {
                        unique.Add(board);
                    }
                }
                PossibleBoards = unique;
            }
        }
    }
}