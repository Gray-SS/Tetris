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
        private Texture _texture;
        private Rectangle _tetrisViewport;

        public TetrisEngine() : base() { }

        public override void Load()
        {
            _texture = new Texture(4, 3);
            _texture.SetData(new BufferValue[]
            {
                new(null, null, CColor.White), new(null, null, CColor.White), new(null, null, null), new(null, null, null),
                new(null, null, CColor.White), new(null, null, CColor.White),  new(null, null, null), new(null, null, null),
                new(null, null, CColor.White), new(null, null, CColor.White), new(null, null, CColor.White), new(null, null, CColor.White)
            });

            _tetrisViewport = new Rectangle(2, 1, ConsoleWidth / 4, ConsoleHeight - 2);
        }

        public override void Update(float dt)
        {
            
        }

        public override void Draw()
        {
            Graphics.Clear(CColor.DarkRed);

            Graphics.DrawTexture(1, 1, _texture);
        }
    }
}
