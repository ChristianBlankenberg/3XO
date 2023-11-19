namespace TicTacToe.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::GameLogic;

    public class Game
    {
        private const Player MinPlayer = Player.Computer;
        private const Player MaxPlayer = Player.Player;
        private const CalculationMethod ComputerCalculationMethod = CalculationMethod.AlphaBetaPrune;

        private Func<Coordinates> getCoordinatesFromInput;
        private Action<List<string>> outputAction;
        private MinMaxLogicClass gameLogicMinMax = null;
        private AlphaBetaPruneClass gameLogicAlphaBetaPrune = null;
        private Random random;
        private IBoard board;
        private GameNeuronalNet gameNeuronal;
        public Player StartPlayer { get; }

        internal Game(ThreeXOBoard board, Player startPlayer)
        {
            this.board = board;
            this.StartPlayer = startPlayer;

            this.random = new Random();
            this.gameLogicMinMax = new MinMaxLogicClass(MinPlayer, MaxPlayer);
            this.gameLogicAlphaBetaPrune = new AlphaBetaPruneClass(MinPlayer, MaxPlayer);

            this.gameNeuronal = new GameNeuronalNet();
            this.gameNeuronal.Init();
        }

        internal void Run(Func<Coordinates> getCoordinatesFromInput, Action<List<string>> outputAction)
        {
            this.getCoordinatesFromInput = getCoordinatesFromInput;
            this.outputAction = outputAction;
            this.PrintBoard();

            while (!this.Over())
            {
                this.Set(this.GetCalculationMethod(this.StartPlayer));
                if (!this.Over())
                {
                    this.Set(this.GetCalculationMethod(this.StartPlayer.Opponent()));
                }
            }
        }

        internal void Test(Action<List<string>> outputAction, Action pauseFinished)
        {
            this.outputAction = outputAction;
            for (int spalte = 0; spalte < 3; spalte++)
            {
                this.Clear();

                foreach (var index in ThreeXOBoard.SpalteIndexes(spalte))
                {
                    this.board.Set(index, Player.Player);
                }

                this.PrintBoard();
                pauseFinished();
            }

            for (int reihe = 0; reihe < 3; reihe++)
            {
                this.Clear();

                foreach (var index in ThreeXOBoard.ReiheIndexes(reihe))
                {
                    this.board.Set(index, Player.Player);
                }

                this.PrintBoard();
                pauseFinished();
            }

            this.Clear();

            foreach (var index in ThreeXOBoard.DiagonaleLIUROIndexes())
            {
                this.board.Set(index, Player.Player);
            }

            this.PrintBoard();
            pauseFinished();

            this.Clear();

            foreach (var index in ThreeXOBoard.DiagonaleLOURUIndexes())
            {
                this.board.Set(index, Player.Player);
            }

            this.PrintBoard();
            pauseFinished();
        }

        private ICoordinates GetCoordinates(CalculationMethod calculationMethod)
        {
            switch (calculationMethod)
            {
                case CalculationMethod.Unknown:
                case CalculationMethod.None:
                case CalculationMethod.Input:
                    return this.getCoordinatesFromInput();
                case CalculationMethod.Random:
                    return this.GetRandomCoordinates(board);
                case CalculationMethod.NeuronalNet:
                    return this.GetCoordinatesFromNeuronalNet();
                case CalculationMethod.MinMax:
                    return new Coordinates(GameLogic.GetMaxValueFieldIdx(board, this.gameLogicMinMax));
                case CalculationMethod.AlphaBetaPrune:
                    return new Coordinates(GameLogic.GetMaxValueFieldIdx(board, this.gameLogicAlphaBetaPrune));
            }

            throw new NotImplementedException();
        }

        private Coordinates GetCoordinatesFromNeuronalNet() => this.gameNeuronal.GetOutput(this.board);

        private CalculationMethod GetCalculationMethod(Player player)
        {
            switch (player)
            {
                case Player.Computer:
                    return ComputerCalculationMethod;
                case Player.Player:
                    return CalculationMethod.Input;
            }

            return CalculationMethod.Random;
        }

        private ICoordinates GetRandomCoordinates(IBoard board)
        {
            var emptyFieldIdxs = board.GetEmptyFieldIdxs();
            return new Coordinates(emptyFieldIdxs[random.Next(0, emptyFieldIdxs.Count)]);
        }

        private bool Over() => this.board.IsFull() ? true : this.board.Winner() != Player.None;

        private void Clear() => this.board = ThreeXOBoard.Empty(this.StartPlayer);

        private bool IsEmpty(int fieldIdx) => this.board.IsEmpty(fieldIdx);

        private void Set(int fieldIdx) => this.board.Set(fieldIdx);

        private void Set(CalculationMethod calculationMethod)
        {
            ICoordinates coordinates = this.GetCoordinates(calculationMethod);
            this.Set(coordinates.FieldNr);
            this.PrintBoard();
        }

        private void PrintBoard() => this.outputAction(this.board.Print());

        private string BoardToString() => this.board.ToString();
    }
}
