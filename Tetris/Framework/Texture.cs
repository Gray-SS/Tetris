using System;

namespace Tetris
{
    public class Texture
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private Glyph[] _data;

        public Texture(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            _data = new Glyph[width * height];
        }

        public void SetData(Glyph[] data)
        {
            if (data.Length != Width * Height)
                throw new Exception();

            _data = data;
        }

        public Glyph[] GetData()
        {
            return _data;
        }
    }
}
