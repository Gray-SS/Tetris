using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace Tetris
{
    public class Buffer
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        private SafeFileHandle h;
        private CharInfo[] buf;
        private SmallRect rect;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        public Buffer(int x, int y, int width, int height)
        {
            h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            this.Width = width;
            this.Height = height;
            this.X = x;
            this.Y = y;
            buf = new CharInfo[this.Width * this.Height];
            rect = new SmallRect();
            rect.setDrawCord((short)X, (short)Y);
            rect.setWindowSize((short)(Width + X), (short)(Height + Y));
            Clear();
        }

        public void Print()
        {
            if (!h.IsInvalid)
            {
                WriteConsoleOutput(h, buf, new Coord((short)Width, (short)Height), new Coord((short)0, (short)0), ref rect);
            }
        }
        
        public void Clear()
        {
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i].Attributes = 1;
                buf[i].Char.AsciiChar = 32;
            }
        }
        
        public void SetBuffer(CharInfo[] b)
        {
            if (b == null)
            {
                throw new System.ArgumentNullException();
            }

            buf = b;
        }
    }
}


[StructLayout(LayoutKind.Sequential)]
public struct Coord
{
    private short X;
    private short Y;

    public Coord(short X, short Y)
    {
        this.X = X;
        this.Y = Y;
    }
};

[StructLayout(LayoutKind.Explicit)]
public struct CharUnion
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(0)]
    public byte AsciiChar;
}

[StructLayout(LayoutKind.Explicit)]
public struct CharInfo
{
    [FieldOffset(0)]
    public CharUnion Char;
    [FieldOffset(2)]
    public short Attributes;
}

[StructLayout(LayoutKind.Sequential)]
public struct SmallRect
{
    private short Left;
    private short Top;
    private short Right;
    private short Bottom;
    public void setDrawCord(short l, short t)
    {
        Left = l;
        Top = t;
    }
    public void setWindowSize(short r, short b)
    {
        Right = r;
        Bottom = b;
    }
}