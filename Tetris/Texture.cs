using System;

namespace Tetris
{
    public class Texture
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private BufferValue[] _data;

        public Texture(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            _data = new BufferValue[width * height];
        }

        public void SetData(BufferValue[] data)
        {
            if (data.Length != Width * Height)
                throw new Exception();

            _data = data;
        }

        public BufferValue[] GetData()
        {
            return _data;
        }
    }
}
