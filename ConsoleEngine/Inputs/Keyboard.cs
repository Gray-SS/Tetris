using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ConsoleEngine.Inputs
{
    public static class Keyboard
    {
        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);

        public static bool IsKeyDown(Keys key)
        {
            return 0 != (GetAsyncKeyState((int)key) & 0x8000);
        }

        public static IEnumerable<Keys> GetKeysPressed()
        {
            foreach (var key in Enum.GetValues(typeof(Keys)))
            {
                if (IsKeyDown((Keys)key))
                {
                    yield return (Keys)key;
                }
            }
        }
    }
}
