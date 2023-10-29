﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic;
using TicTacToe.GameLogic;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConsoleGame consoleGame = new ConsoleGame(new Game(Board.Empty()));
            //consoleGame.Debug();

            //GameNeuronalNet gameNeuronalNet = new GameNeuronalNet();
            //gameNeuronalNet.Init();
            //var outputs = gameNeuronalNet.Test();
            //foreach(var output in outputs)
            //{
            //    Console.WriteLine(output);
            //}

            //Console.ReadKey();

            //GameNeuronalNet gameNeuronalNet = new GameNeuronalNet();
            //gameNeuronalNet.Train();

            //ConsoleGame consoleGame = new ConsoleGame(new Game(Board.Empty()));
            //consoleGame.Test();
            //consoleGame.Run();

            //Console.ReadLine();

            QLearnLogic qLearnLogic = new QLearnLogic();
            qLearnLogic.QLearn(new Board(" ; ; ; ;X;X; ; ;O"), Player.Computer);
        }
    }
}
