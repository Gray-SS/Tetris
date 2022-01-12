using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Tetris
{
    public abstract class Engine
    {
        public string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }
        public Graphics Graphics { get; private set; }
        public Viewport Viewport { get; private set; }
        public int ConsoleWidth { get; private set; }
        public int ConsoleHeight { get; private set; }

        private Thread _updateThread;
        private Thread _renderThread;

        public Engine()
        {
            this.Title = Assembly.GetExecutingAssembly().GetName().Name;
            this.Viewport = new Viewport(0, 0, Console.WindowWidth, Console.WindowHeight);
            this.Graphics = new Graphics(this);
            this.ConsoleWidth = Console.WindowWidth;
            this.ConsoleHeight = Console.WindowHeight;

            _updateThread = new Thread(Update);
            _renderThread = new Thread(Render);
        }

        public void Run()
        {
            this.ApplyChanges();

            _updateThread.Start();
            _renderThread.Start();
            this.Load();
        }

        public void ApplyChanges()
        {
            Console.WindowWidth = this.ConsoleWidth;
            Console.WindowHeight = this.ConsoleHeight;
            Console.SetBufferSize(ConsoleWidth, ConsoleHeight);
        }

        public virtual void Load() { }
        public virtual void Update(float dt) { }
        public virtual void Draw() { }


        private void Update()
        {
            float elapsedSeconds = 0f;

            while (_updateThread.IsAlive)
            {
                Stopwatch sw = Stopwatch.StartNew();
                Update(elapsedSeconds);
                Thread.Sleep(1);
                elapsedSeconds = (float)sw.Elapsed.TotalSeconds;
            }
        }

        private void Render()
        {
            while (_renderThread.IsAlive)
            {
                Console.CursorVisible = false;

                this.Draw();

                Graphics.Render();

                Thread.Sleep(1);
            }
        }
    }

    public struct BufferValue
    {
        public char Char { get; set; }
        public CColor Color { get; set; }
        public CColor BackgroundColor { get; set; }

        public BufferValue(char? c, CColor? color, CColor? backgroundColor)
        {
            this.Char = c ?? ' ';
            this.Color = color ?? CColor.Transparent;
            this.BackgroundColor = backgroundColor ?? CColor.Transparent;
        }
    }
}
