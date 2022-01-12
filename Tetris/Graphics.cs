using System;

namespace Tetris
{
    public class Graphics
    {
        private Engine _engine;
        private BufferValue[,] _map;
        private Buffer _buffer;

        public Graphics(Engine engine)
        {
            _engine = engine;

            var vp = _engine.Viewport;
            _map = new BufferValue[vp.Width, vp.Height];
            _buffer = new Buffer(vp.X, vp.Y, vp.Width, vp.Height);
        }

        public void DrawText(int x, int y, string text, CColor? color, CColor? backgroundColor)
        {
            for (int i = 0; i < text.Length; i++)
            {
                SetPixel(x + i, y, text[i], color, backgroundColor);
            }
        }

        public void DrawTexture(int x, int y, Texture texture)
        {
            var data = texture.GetData();

            for(int i = 0; i < texture.Width; i++)
            {
                for (int j = 0; j < texture.Height; j++)
                {
                    var value = data[j * texture.Width + i];
                    this.SetPixel(x + i, y + j, value);
                }
            }
        }

        public void DrawRectangle(Rectangle rectangle, CColor? backgroundColor) => DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, backgroundColor);
        public void DrawRectangle(Rectangle rectangle, char? c, CColor? color, CColor? backgroundColor) => DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, c, color, backgroundColor);
        public void DrawRectangle(int x, int y, int width, int height, CColor? backgroundColor) => DrawRectangle(x, y, width, height, null, null, backgroundColor);
        public void DrawRectangle(int x, int y, int width, int height, char? c, CColor? color, CColor? backgroundColor)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    this.SetPixel(x + i, y + j, c, color, backgroundColor);
                }
            }
        }

        private void SetPixel(int x, int y, char? c, CColor? color, CColor? backgroundColor) => SetPixel(x, y, new BufferValue(c, color, backgroundColor));
        private void SetPixel(int x, int y, BufferValue value)
        {
            if (x < 0 || x >= _map.GetLength(0) || y < 0 || y >= _map.GetLength(1))
                return;

            if (value.Color == CColor.Transparent)
                value.Color = _map[x, y].Color;

            if (value.BackgroundColor == CColor.Transparent)
                value.BackgroundColor = _map[x, y].BackgroundColor;

            _map[x, y] = value;
        }

        public void Clear() => Clear(null, null, CColor.White);
        public void Clear(CColor? backgroundColor) => Clear(null, null, backgroundColor);
        public void Clear(char? c, CColor? color, CColor? backgroundColor) => Clear(new BufferValue(c, color, backgroundColor));
        public void Clear(BufferValue value)
        {
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    SetPixel(i, j, value);
                }
            }
        }

        public void Render()
        {
            var vp = _engine.Viewport;
            int c = 0;
            CharInfo[] _charInfo = new CharInfo[vp.Width * vp.Height];
            for (int j = 0; j < vp.Height; j++)
            {
                for (int i = 0; i < vp.Width; i++)
                {
                    var v = _map[i, j];
                    _charInfo[c] = new CharInfo() { Attributes = (short)((int)v.Color | ((int)v.BackgroundColor) << 4), Char = new CharUnion() { AsciiChar = (byte)v.Char, UnicodeChar = v.Char} };
                    c++;
                }

            }

            _buffer.SetBuffer(_charInfo);
            _buffer.Print();
        }
    }
}