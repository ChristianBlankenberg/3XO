﻿
namespace TicTacToe
{
    using System;
    using System.IO;
    using TicTacToe.GameLogic;

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

                this.Set(Player.Player, CalculationMethod.Console);
                this.Set(Player.Computer, CalculationMethod.NeuronalNet);
            }
        }

        private void Set(Player playerOrComputer, CalculationMethod calculationMethod)
        {
            Coordinates coordinates;
            do
            {
                coordinates = this.GetCoordinates(playerOrComputer, calculationMethod);
            }

            while (!this.game.IsEmpty(coordinates));
            this.game.Set(coordinates, playerOrComputer);
            this.PrintBoard();
        }

        private Coordinates GetCoordinates(Player playerOrComputer, CalculationMethod calculationMethod)
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
            }

            throw new NotImplementedException();
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
