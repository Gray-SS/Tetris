using System;
using System.Collections.Generic;

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

    public class TetrisEngine : Engine
    {
        private Tile[,] _tetris_sandbox;
        private Rectangle _tetrisViewport;

        public TetrisEngine() : base() { }

        public override void Load()
        {
            _tetrisViewport = new Rectangle(2, 1, 19, ConsoleHeight - 2);
            _tetris_sandbox = new Tile[_tetrisViewport.Width, _tetrisViewport.Height];

            for (int i = 0; i < _tetris_sandbox.GetLength(1); i++)
            {
                for (int j = 0; j < _tetris_sandbox.GetLength(0); j++)
                {
                    _tetris_sandbox[j, i] = new Tile() { X = j + _tetrisViewport.X, Y = i + _tetrisViewport.Y, Value = new BufferValue(null, null, CColor.Gray) };
                }
            }
        }
        
        public override void Update(float dt)
        {
            
        }

        public override void Draw()
        {
            Graphics.Clear(CColor.Black);

            Graphics.DrawRectangle(1, 0, 20, 1, null, null, CColor.DarkGray);
            Graphics.DrawRectangle(0, 0, 2, ConsoleHeight, null, null, CColor.DarkGray);
            Graphics.DrawRectangle(1, ConsoleHeight - 1, 20, 1, null, null, CColor.DarkGray);
            Graphics.DrawRectangle(21, 0, 2, ConsoleHeight, null, null, CColor.DarkGray);

            foreach (var tile in _tetris_sandbox)
            {
                tile.Draw(Graphics);
            }

            Graphics.DrawText(ConsoleWidth - 20, 1, $"FPS: {FPS}", CColor.Red, null);
            Graphics.DrawText(ConsoleWidth - 20, 2, $"Console Width: {ConsoleWidth}", CColor.Green, null);
            Graphics.DrawText(ConsoleWidth - 20, 3, $"Console Height: {ConsoleHeight}", CColor.Blue, null);
        }
    }

    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public BufferValue Value { get; set; }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(X, Y, 1, 1, Value.Char, Value.Color, Value.BackgroundColor);
        }
    }
}
