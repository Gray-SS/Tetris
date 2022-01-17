using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ConsoleEngine.Graphics;
using ConsoleEngine.Helper;

namespace ConsoleEngine
{
    public abstract class Engine
    {
        public string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        public GraphicsDevice Graphics { get; private set; }
        public Viewport Viewport { get; private set; }
        public int ConsoleWidth { get; private set; }
        public int ConsoleHeight { get; private set; }
        public int AspectRatio { get; private set; } = 2;
        public int FPS { get; private set; }

        private readonly IntPtr stdInputHandle = NativeMethods.GetStdHandle(-10);
        private readonly IntPtr stdOutputHandle = NativeMethods.GetStdHandle(-11);

        private Thread _updateThread;
        private Thread _renderThread;

        public Engine()
        {
            this.Title = Assembly.GetExecutingAssembly().GetName().Name;
            this.ConsoleWidth = Console.WindowWidth;
            this.ConsoleHeight = Console.WindowHeight * 2;
            this.Viewport = new Viewport(0, 0, ConsoleWidth, ConsoleHeight);
            this.Graphics = new GraphicsDevice(this);

            NativeMethods.SetConsoleMode(stdInputHandle, 0x0080);

            ConsoleFont.SetFont(stdOutputHandle, 10, 10);

            _updateThread = new Thread(Update);
            _renderThread = new Thread(Render);
        }

        public void Construct(int bufferWidth, int bufferHeight, int fontWidth, int fontHeight)
        {
            this.ConsoleWidth = bufferWidth;
            this.ConsoleHeight = bufferHeight;

            this.Viewport = new Viewport(0, 0, ConsoleWidth, ConsoleHeight);
            this.Graphics = new GraphicsDevice(this);
            ConsoleFont.SetFont(stdOutputHandle, (short)fontWidth, (short)fontHeight);
        }

        public void Run()
        {
            this.ApplyChanges();

            this.Load();
            _updateThread.Start();
            _renderThread.Start();
        }

        public void ApplyChanges()
        {
            Console.WindowWidth = this.ConsoleWidth;
            Console.WindowHeight = this.ConsoleHeight;
            Console.SetBufferSize(ConsoleWidth, ConsoleHeight);

            this.Viewport = new Viewport(0, 0, ConsoleWidth, ConsoleHeight);
        }

        public virtual void Load() { }
        public virtual void Update(float dt) { }
        public virtual void Draw() { }

        public static void Wait(int miliseconds)
        {
            Task.Delay(miliseconds).Wait();
        }

        public static void Wait(Task task)
        {
            task.Wait();
            return;
        }

        public static TResult Wait<TResult>(Task<TResult> task)
        {
            task.Wait();
            return task.Result;
        }

        public static void WaitUntil(Func<bool> func)
        {
            do { Wait(10); }
            while (!func.Invoke());
        }

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
            int _framesCount = 0;
            Task.Run(() =>
            {
                while (_renderThread.IsAlive)
                {
                    Thread.Sleep(1000);
                    FPS = _framesCount;
                    _framesCount = 0;
                }
            });

            while (_renderThread.IsAlive)
            {
                Console.CursorVisible = false;

                this.Draw();

                Graphics.Render();

                Thread.Sleep(1);
                _framesCount++;
            }
        }
    }

    public struct Glyph
    {
        public char Char { get; set; }
        public CColor Color { get; set; }
        public CColor BackgroundColor { get; set; }

        public Glyph(char? c, CColor? color, CColor? backgroundColor)
        {
            this.Char = c ?? ' ';
            this.Color = color ?? CColor.Transparent;
            this.BackgroundColor = backgroundColor ?? CColor.Transparent;
        }
    }
}
