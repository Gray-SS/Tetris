using System;

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
        private Rectangle _tetrisViewport;

        public TetrisEngine() : base() { }

        public override void Load()
        {
            _tetrisViewport = new Rectangle(2, 1, ConsoleWidth / 4, ConsoleHeight - 2);
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

            Graphics.DrawText(ConsoleWidth - 20, 1, $"FPS: {FPS}", CColor.Red, null);
            Graphics.DrawText(ConsoleWidth - 20, 2, $"Console Width: {ConsoleWidth}", CColor.Green, null);
            Graphics.DrawText(ConsoleWidth - 20, 3, $"Console Height: {ConsoleHeight}", CColor.Blue, null);
        }
    }
}
