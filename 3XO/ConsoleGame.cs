
namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::GameLogic;
    using TicTacToe.GameLogic;

    internal class ConsoleGame<BoardType> where BoardType : class, IBoard
    {
        private CalculationMethod ComputerCalculationMethod = CalculationMethod.MinMax;
        private GameLogicMinMax gameLogicMinMax = null;
        private GameLogicAlphaBetaPrune gameLogicAlphaBetaPrune = null;
        private Game game;

        internal ConsoleGame(Game game)
        {
            this.game = game;
        }

        internal void PrintBoard()
        {
            Console.Clear();

            var output = this.game.PrintBoard();
            foreach (var line in output)
            {
                Console.WriteLine(line);
            }
        }

        internal void Test()
        {
            for (int spalte = 0; spalte < 3; spalte++)
            {
                this.game.Clear();

                foreach (var index in ThreeXOBoard.SpalteIndexes(spalte))
                {
                    this.game.Set(index, Player.Player);
                }

                this.PrintBoard();
                Console.ReadKey();
            }

            for (int reihe = 0; reihe < 3; reihe++)
            {
                this.game.Clear();

                foreach (var index in ThreeXOBoard.ReiheIndexes(reihe))
                {
                    this.game.Set(index, Player.Player);
                }

                this.PrintBoard();
                Console.ReadKey();
            }

            this.game.Clear();

            foreach (var index in ThreeXOBoard.DiagonaleLIUROIndexes())
            {
                this.game.Set(index, Player.Player);
            }

            this.PrintBoard();
            Console.ReadKey();

            this.game.Clear();

            foreach (var index in ThreeXOBoard.DiagonaleLOURUIndexes())
            {
                this.game.Set(index, Player.Player);
            }

            this.PrintBoard();
            Console.ReadKey();
        }

        internal void Run(IBoard board, Player startPlayer)
        {
            this.game.SetBoard(board);
            this.PrintBoard();

            this.gameLogicMinMax = new GameLogicMinMax(Player.Computer, Player.Player);

            this.gameLogicAlphaBetaPrune = new GameLogicAlphaBetaPrune(
                new MinMaxDescriptionAlphaBetaPrune(
                    Player.Player,
                    int.MinValue,
                    (val1, val2) => Math.Max(val1, val2),
                    (list) => list.Max(x => x.value),
                    true,
                    false),
                new MinMaxDescriptionAlphaBetaPrune(
                    Player.Computer,
                    int.MaxValue,
                    (val1, val2) => Math.Min(val1, val2),
                    (list) => list.Min(x => x.value),
                    false,
                    true));

            while (!this.game.Over())
            {
                this.Set(this.game.GetBoard(), startPlayer, this.GetCalculationMethod(startPlayer));
                if (!this.game.Over())
                {
                    this.Set(this.game.GetBoard(), startPlayer.Opponent(), this.GetCalculationMethod(startPlayer.Opponent()));
                }
            }
        }

        private CalculationMethod GetCalculationMethod(Player player)
        {
            switch (player)
            {
                case Player.Computer:
                    return ComputerCalculationMethod;
                case Player.Player:
                    return CalculationMethod.Console;
            }

            return CalculationMethod.Random;
        }

        private void Set(IBoard board, Player playerOrComputer, CalculationMethod calculationMethod)
        {
            ICoordinates coordinates = this.GetCoordinates(board, playerOrComputer, calculationMethod);            
            this.game.Set(coordinates.FieldNr, playerOrComputer);
            this.PrintBoard();
        }

        private ICoordinates GetCoordinates(IBoard board, Player playerOrComputer, CalculationMethod calculationMethod)
        {
            switch (calculationMethod)
            {
                case CalculationMethod.Unknown:
                case CalculationMethod.None:
                case CalculationMethod.Console:
                    return this.GetCoordinatesFromConsole();
                case CalculationMethod.Random:
                    return this.GetRandomCoordinates(board);
                case CalculationMethod.NeuronalNet:
                    return this.GetCoordinatesFromNeuronalNet();
                case CalculationMethod.QValues:
                    return this.GetCoordinatesQValues(playerOrComputer, board);
                case CalculationMethod.MinMax:
                    return new Coordinates(this.gameLogicMinMax.GetFavouriteFieldIdx(board, playerOrComputer));
                case CalculationMethod.AlphaBetaPrune:
                    return new Coordinates(this.gameLogicAlphaBetaPrune.GetFavouriteFieldIdx(board, playerOrComputer));
            }

            throw new NotImplementedException();
        }

        List<QualityDescription<BoardType>> boardsAndQValues;
        private ICoordinates GetCoordinatesQValues(Player playerOrComputer, IBoard board)
        {
            if (this.boardsAndQValues == null)
            {
                this.boardsAndQValues = QualityDescription<BoardType>.GetQualityDexcriptionList("boardsAndQValues.xml");
            }

            var previewBoard = this.boardsAndQValues.FirstOrDefault(bq => bq.Board.Equals(board));
            if (previewBoard == null)
            {
                throw new ArithmeticException();
            }

            int maxIdx = -1;

            // Test for loose winning
            var checkBoard = previewBoard.Board.Copy();
            var emptyFieldIdxs = checkBoard.GetEmptyFieldIdxs();
            foreach (var emptyFieldIdx in emptyFieldIdxs)
            {
                checkBoard.Set(emptyFieldIdx, playerOrComputer);
                if (checkBoard.Winner() == playerOrComputer)
                {
                    maxIdx = emptyFieldIdx;
                }

                checkBoard.Set(emptyFieldIdx, Player.None);
            }

            // Test for loose blocking
            if (maxIdx == -1)
            {
                checkBoard = previewBoard.Board.Copy();
                foreach (var emptyFieldIdx in emptyFieldIdxs)
                {
                    checkBoard.Set(emptyFieldIdx, playerOrComputer.Opponent());
                    if (checkBoard.Winner() == playerOrComputer.Opponent())
                    {
                        maxIdx = emptyFieldIdx;
                    }

                    checkBoard.Set(emptyFieldIdx, Player.None);
                }
            }

            if (maxIdx == -1)
            {
                List<IBoard> boards = new List<IBoard>();
                this.GetNextTwoBoardPossibilities(previewBoard.Board, boards, playerOrComputer, 0);

                long maxValue = long.MinValue;
                List<int> possibleFields = new List<int>();

                foreach (var b in boards)
                {
                    var previewB = this.boardsAndQValues.FirstOrDefault(bq => bq.Board.Equals(b));
                    if (previewB == null)
                    {
                        throw new ArithmeticException();
                    }

                    foreach (var emptyFieldIdx in emptyFieldIdxs)
                    {
                        var q = previewB.WinsLosses[emptyFieldIdx].SplitWinBoardsQ() - previewB.WinsLosses[emptyFieldIdx].SplitLooseBoardsQ();
                        if (q > maxValue)
                        {
                            possibleFields = new List<int>();
                            possibleFields.Add(emptyFieldIdx);
                            maxValue = q;
                        }
                        else if (q == maxValue && !possibleFields.Contains(emptyFieldIdx))
                        {
                            possibleFields.Add(emptyFieldIdx);
                        }
                    }
                }

                Random random = new Random();

                maxIdx = possibleFields[random.Next(0, possibleFields.Count())];
            }

            return new Coordinates(maxIdx);
        }

        private void GetNextTwoBoardPossibilities(IBoard board, List<IBoard> boards, Player playerOrComputer, int depth)
        {
            if (depth < 2)
            {
                for (int idx = 0; idx < 9; idx++)
                {
                    if (board.Get(idx) == Player.None)
                    {
                        IBoard newBoard = board.Copy();
                        newBoard.Set(idx, playerOrComputer);
                        boards.Add(newBoard);
                        this.GetNextTwoBoardPossibilities(newBoard, boards, playerOrComputer.Opponent(), depth + 1);
                    }
                }
            }
        }

        private ICoordinates GetRandomCoordinates(IBoard board)
        {
            var emptyFieldIdxs = board.GetEmptyFieldIdxs();

            Random random = new Random();
            return new Coordinates(emptyFieldIdxs[random.Next(0, emptyFieldIdxs.Count)]);
        }

        private Coordinates GetCoordinatesFromNeuronalNet() => this.game.GetCoordinatesFromNeuronalNet();

        private Coordinates GetCoordinatesFromConsole()
        {
            ConsoleKeyInfo spalte;
            do
            {
                spalte = Console.ReadKey();
            }
            while (!this.Is0(spalte) && !this.Is1(spalte) && !this.Is2(spalte));

            ConsoleKeyInfo reihe;
            do
            {
                reihe = Console.ReadKey();
            }
            while (!this.Is0(reihe) && !this.Is1(reihe) && !this.Is2(reihe));

            return new Coordinates(this.GetNr(spalte), this.GetNr(reihe));
        }

        private bool Is0(ConsoleKeyInfo consoleKeyInfo) => consoleKeyInfo.Key == ConsoleKey.D0 || consoleKeyInfo.Key == ConsoleKey.NumPad0;

        private bool Is1(ConsoleKeyInfo consoleKeyInfo) => consoleKeyInfo.Key == ConsoleKey.D1 || consoleKeyInfo.Key == ConsoleKey.NumPad1;

        private bool Is2(ConsoleKeyInfo consoleKeyInfo) => consoleKeyInfo.Key == ConsoleKey.D2 || consoleKeyInfo.Key == ConsoleKey.NumPad2;

        private int GetNr(ConsoleKeyInfo consoleKeyInfo)
        {
            return
                this.Is0(consoleKeyInfo) ? 0 :
                    this.Is1(consoleKeyInfo) ? 1 :
                        this.Is2(consoleKeyInfo) ? 2 :
                            -1;
        }
    }
}
