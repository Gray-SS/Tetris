using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MenuDeLaMortQuiTue
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowMenu(new string[] {
                "Test1",
                "Test2",
                "Test3",
                "Test4",
            });
        }

        static void ShowMenu(string[] questions)
        {
            int offsetX = 5;
            int offsetY = 2;

            int selection = 0;
            int selectionOffset = 2;

            int selectionCharRotationDelay = 200;
            char selectionCharRotation = '+';
            Task.Run(() =>
            {
                int i = 0;
                List<char> _loop = new List<char>() { '+', 'x', '=', 'x', };
                while (true)
                {
                    selectionCharRotation = _loop[i % _loop.Count];
                    Thread.Sleep(selectionCharRotationDelay);
                    i++;
                }
            });

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.UpArrow:
                            selection -= 1;
                            if (selection < 0) selection = questions.Length - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            selection += 1;
                            if (selection >= questions.Length) selection = questions.Length - 1;
                            break;
                    }
                }

                Console.CursorVisible = false;
                for (int i = 0; i < questions.Length; i++)
                {
                    string a = string.Empty;
                    for (int j = 0; j < questions[i].Length + selectionOffset; j++)
                        a += " ";

                    int crntSelectOffset = 0;

                    ConsoleColor color;
                    if (selection == i)
                    {
                        color = ConsoleColor.Magenta;
                        crntSelectOffset = selectionOffset;
                    }
                    else color = ConsoleColor.White;

                    Console.SetCursorPosition(offsetX, offsetY + i);
                    Console.WriteLine(a);

                    if (selection == i)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.SetCursorPosition(offsetX, offsetY + i);
                        Console.Write(selectionCharRotation);
                        Console.ResetColor();
                    }

                    Console.ForegroundColor = color;
                    Console.SetCursorPosition(offsetX + crntSelectOffset, offsetY + i);
                    Console.WriteLine(questions[i]);
                    Console.ResetColor();
                }

                Thread.Sleep(1);
            }
        }
    }
}
