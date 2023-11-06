
namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using global::GameLogic;
    using TicTacToe.GameLogic;

    // X; ;O;O;X;X;X; ;O

    internal class ConsoleGame
    {
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

        internal void Debug()
        {
            this.game.Debug();
        }

        internal void Test()
        {
            for (int spalte = 0; spalte < 3; spalte++)
            {
                this.game.Clear();

                foreach (var index in Board.SpalteIndexes(spalte))
                {
                    this.game.Set(new Coordinates(index), Player.Player);
                }

                this.PrintBoard();
                Console.ReadKey();
            }

            for (int reihe = 0; reihe < 3; reihe++)
            {
                this.game.Clear();

                foreach (var index in Board.ReiheIndexes(reihe))
                {
                    this.game.Set(new Coordinates(index), Player.Player);
                }

                this.PrintBoard();
                Console.ReadKey();
            }

            this.game.Clear();

            foreach (var index in Board.DiagonaleLIUROIndexes())
            {
                this.game.Set(new Coordinates(index), Player.Player);
            }

            this.PrintBoard();
            Console.ReadKey();

            this.game.Clear();

            foreach (var index in Board.DiagonaleLOURUIndexes())
            {
                this.game.Set(new Coordinates(index), Player.Player);
            }

            this.PrintBoard();
            Console.ReadKey();
        }

        internal void Run()
        {
            this.game.Clear();
            this.PrintBoard();

            while (!this.game.Over())
            {
                this.Set(Player.Player, CalculationMethod.Console, this.game.GetBoard());
                if (!this.game.Over())
                {
                    this.Set(Player.Computer, CalculationMethod.QValues, this.game.GetBoard());
                }
            }
        }

        private void Set(Player playerOrComputer, CalculationMethod calculationMethod, Board board)
        {
            Coordinates coordinates;
            do
            {
                coordinates = this.GetCoordinates(playerOrComputer, calculationMethod, board);
            }

            while (!this.game.IsEmpty(coordinates));
            this.game.Set(coordinates, playerOrComputer);
            this.PrintBoard();
        }

        private Coordinates GetCoordinates(Player playerOrComputer, CalculationMethod calculationMethod, Board board)
        {
            switch (calculationMethod)
            {
                case CalculationMethod.Unknown:
                case CalculationMethod.None:
                case CalculationMethod.Console:
                    return this.GetCoordinatesFromConsole();
                case CalculationMethod.Random:
                    return this.GetRandomCoordinates();
                case CalculationMethod.NeuronalNet:
                    return this.GetCoordinatesFromNeuronalNet();
                case CalculationMethod.QValues:
                    return this.GetCoordinatesQValues(playerOrComputer, board);
            }

            throw new NotImplementedException();
        }

        //List<QualityDescription> playerBoardsAndQValues = null;
        //List<QualityDescription> computerBoardsAndQValues = null;
        List<QualityDescription> boardsAndQValues;
        private Coordinates GetCoordinatesQValues(Player playerOrComputer, Board board)
        {
            if (this.boardsAndQValues == null)
            {
                this.boardsAndQValues = QualityDescription.GetQualityDexcriptionList("boardsAndQValues.xml");
            }

            var previewBoard = this.boardsAndQValues.FirstOrDefault(bq => bq.Board == board);
            if (previewBoard == null)
            {
                throw new ArithmeticException();
            }

            int maxIdx = -1;

            // Test for loose winning
            var checkBoard = previewBoard.Board.Copy();
            var fields = checkBoard.Fields();
            for (int idx = 0; idx < fields.Count && maxIdx == -1; idx++)
            {
                if (fields[idx] == Player.None)
                {
                    checkBoard.Set(idx, playerOrComputer);
                    if (checkBoard.Winner() == playerOrComputer)
                    {
                        maxIdx = idx;
                    }

                    checkBoard.Set(idx, Player.None);
                }
            }

            // Test for loose blocking
            if (maxIdx == -1)
            {
                checkBoard = previewBoard.Board.Copy();
                fields = checkBoard.Fields();
                for (int idx = 0; idx < fields.Count && maxIdx == -1; idx++)
                {
                    if (fields[idx] == Player.None)
                    {
                        checkBoard.Set(idx, playerOrComputer.Opponent());
                        if (checkBoard.Winner() == playerOrComputer.Opponent())
                        {
                            maxIdx = idx;
                        }

                        checkBoard.Set(idx, Player.None);
                    }
                }
            }

            if (maxIdx == -1)
            {
                List<Board> boards = new List<Board>();
                this.GetNextTwoBoardPossibilities(previewBoard.Board, boards, playerOrComputer, 0);

                long maxValue = long.MinValue;

                foreach(var b in boards)
                {
                    var previewB = this.boardsAndQValues.FirstOrDefault(bq => bq.Board == b);
                    if (previewB == null)
                    {
                        throw new ArithmeticException();
                    }

                    for (int idx = 0; idx < fields.Count; idx++)
                    {
                        if (fields[idx] == Player.None)
                        {
                            if (previewB.WinsLosses[idx].SplitLooseBoardsQ() > maxValue)
                            {
                                maxIdx = idx;
                                maxValue = previewB.WinsLosses[idx].SplitWinBoardsQ();
                            }
                        }
                    }
                }
            }

            return new Coordinates(maxIdx);
        }

        private void GetNextTwoBoardPossibilities(Board board, List<Board> boards, Player playerOrComputer, int depth)
        {
            if (depth < 2)
            {
                for (int idx = 0; idx < 9; idx++)
                {
                    if (board.Get(idx) == Player.None)
                    {
                        Board newBoard = board.Copy();
                        newBoard.Set(idx, playerOrComputer);
                        boards.Add(newBoard);
                        this.GetNextTwoBoardPossibilities(newBoard, boards, playerOrComputer.Opponent(), depth + 1);
                    }
                }
            }
        }

        private Coordinates GetRandomCoordinates()
        {
            Random random = new Random();
            return new Coordinates(random.Next(0, 3), random.Next(0, 3));
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
