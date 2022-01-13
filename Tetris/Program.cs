using System;
using System.Collections.Generic;
using Tetris.Game;

namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
            TetrisEngine engine = new TetrisEngine();
            engine.Run();
        }
    }
}
