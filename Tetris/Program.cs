using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    class Program
    {
        static int viewportWidth = 24;
        static int viewportHeight = Console.WindowHeight - 5;
        static Buffer buffer;
        static BufferValue[,] map;
        static Random r = new Random();

        static void Main(string[] args)
        {
            int offsetX = Console.WindowWidth / 2 - viewportWidth / 2;
            int offsetY = Console.WindowHeight / 2 - viewportHeight / 2;
            
            buffer = new(offsetX, offsetY, viewportWidth, viewportHeight);
            map = new BufferValue[viewportWidth, viewportHeight];

            Task.Run(() =>
            {
                float elapsedSeconds = 0f;
                while (true)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    Update(elapsedSeconds);
                    Thread.Sleep(1);
                    elapsedSeconds = (float)sw.Elapsed.TotalSeconds;
                }
            });

            while (true)
            {
                Console.CursorVisible = false;
                Clear(new BufferValue(' ', ConsoleColor.White));

                Draw();
                
                int c = 0;
                CharInfo[] _charInfo = new CharInfo[viewportWidth * viewportHeight];
                for (int j = 0; j < viewportHeight; j++)
                {
                    for (int i = 0; i < viewportWidth; i++)
                    {
                        var v = map[i, j];
                        _charInfo[c] = new CharInfo() { Attributes = (short)v.Color, Char = new CharUnion() { AsciiChar = (byte)v.Char, UnicodeChar = v.Char } };
                        c++;
                    }

                }
                buffer.SetBuffer(_charInfo);

                buffer.Print();

                Thread.Sleep(1);
            }
        }

        static bool started;
        static int posX;
        static int posY;
        static float tick = 0.8f;
        static ConsoleColor color;
        static float _timer;
        static void Update(float dt)
        {
            if (!started)
            {
                posX = 4;
                posY = 0;
                color = ConsoleColor.Red;
                started = true;
            }
            else
            {
                _timer += dt;

                if (_timer > tick)
                {
                    _timer = 0f;
                    posY += 1;
                    if (Console.KeyAvailable)
                    {
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.RightArrow: posX += 1; break;
                            case ConsoleKey.LeftArrow: posX -= 1; break;
                        }
                    }
                }
            }
        }

        static void Draw()
        {
            BufferValue wall = new('x', ConsoleColor.Gray);
            DrawColumn(0, 2, wall);
            DrawColumn(viewportWidth-2, 2, wall);
            DrawRow(viewportHeight - 1, 1, wall);

            for (int i = 0; i < 4; i++)
            {
                SetPixel(posX, posY + i, new BufferValue((char)178, color));
            }
        }

        static void DrawRow(int y, int length, BufferValue value)
        {
            for (int j = 0; j < length; j++)
            {
                for (int i = 0; i < viewportWidth; i++)
                {
                    SetPixel(i, y + j, value);
                }
            }
        }

        static void DrawColumn(int x, int length, BufferValue value)
        {
            for (int j = 0; j < length; j++)
            {
                for (int i = 0; i < viewportHeight; i++)
                {
                    SetPixel(x + j, i, value);
                }
            }
        }

        static void SetPixel(int x, int y, BufferValue value)
        {
            if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
                return;

            map[x, y] = value;
        }

        static void Clear(BufferValue value)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = value;
                }
            }
        }

        struct BufferValue
        {
            public char Char { get; set; }
            public ConsoleColor Color { get; set; }

            public BufferValue(char _char, ConsoleColor color)
            {
                this.Char = _char;
                this.Color = color;
            }
        }
    }
}
