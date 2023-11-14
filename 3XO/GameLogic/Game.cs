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
                this.Set(this.StartPlayer, this.GetCalculationMethod(this.StartPlayer));
                if (!this.Over())
                {
                    this.Set(this.StartPlayer.Opponent(), this.GetCalculationMethod(this.StartPlayer.Opponent()));
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
                    this.Set(index, Player.Player);
                }

                this.PrintBoard();
                pauseFinished();
            }

            for (int reihe = 0; reihe < 3; reihe++)
            {
                this.Clear();

                foreach (var index in ThreeXOBoard.ReiheIndexes(reihe))
                {
                    this.Set(index, Player.Player);
                }

                this.PrintBoard();
                pauseFinished();
            }

            this.Clear();

            foreach (var index in ThreeXOBoard.DiagonaleLIUROIndexes())
            {
                this.Set(index, Player.Player);
            }

            this.PrintBoard();
            pauseFinished();

            this.Clear();

            foreach (var index in ThreeXOBoard.DiagonaleLOURUIndexes())
            {
                this.Set(index, Player.Player);
            }

            this.PrintBoard();
            pauseFinished();
        }

        private void Set(Player playerOrComputer, CalculationMethod calculationMethod)
        {
            ICoordinates coordinates = this.GetCoordinates(playerOrComputer, calculationMethod);
            this.Set(coordinates.FieldNr, playerOrComputer);
            this.PrintBoard();
        }

        private ICoordinates GetCoordinates(Player playerOrComputer, CalculationMethod calculationMethod)
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
                    return new Coordinates(this.GetMaxValueFieldIdx(board, playerOrComputer, this.gameLogicMinMax));
                //return new Coordinates(this.gameLogicMinMax.GetFavouriteFieldIdx(board, playerOrComputer));
                case CalculationMethod.AlphaBetaPrune:
                    return new Coordinates(this.GetMaxValueFieldIdx(board, playerOrComputer, this.gameLogicAlphaBetaPrune));
                    //return new Coordinates(this.gameLogicAlphaBetaPrune.GetFavouriteFieldIdx(board, playerOrComputer));
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

        private int GetMaxValueFieldIdx(IBoard board, Player player, IMinMaxValueLogicClass minMaxValueLogicClass)
        {
            var emptyFieldIdxs = board.GetEmptyFieldIdxs();
            List<(int idx, double value)> idxAndValue = new List<(int idx, double value)>();

            foreach (var emptyFieldIdx in emptyFieldIdxs)
            {
                board.Set(emptyFieldIdx, player);
                idxAndValue.Add((emptyFieldIdx, minMaxValueLogicClass.GetValue(board, player.Opponent(), 0)));
                board.Set(emptyFieldIdx, Player.None);
            }

            var minMaxValue = player == MaxPlayer ? idxAndValue.Max(x => x.value) : player == MinPlayer ? idxAndValue.Min(x => x.value) : 0;
            var minMaxValuesAndIdxs = idxAndValue.Where(x => x.value == minMaxValue).ToList();

            return minMaxValuesAndIdxs[random.Next(0, minMaxValuesAndIdxs.Count)].idx;
        }

        private bool Over() => this.board.IsFull() ? true : this.board.Winner() != Player.None;

        private void Clear() => this.board = ThreeXOBoard.Empty(this.StartPlayer);

        private bool IsEmpty(int fieldIdx) => this.board.IsEmpty(fieldIdx);

        private void Set(int fieldIdx, Player playerOrComputer) => this.board.Set(fieldIdx, playerOrComputer);

        private void PrintBoard() => this.outputAction(this.board.Print());

        private string BoardToString() => this.board.ToString();
    }
}
