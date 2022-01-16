using ConsoleEngine;

namespace Tetris.Game.Blocks
{
    public class JBlock : Block
    {
        private readonly Vector2[][] _tiles = new Vector2[][]
        {
            new Vector2[] { new(0,0), new(0,1), new(1,1), new(2,1) },
            new Vector2[] { new(1,0), new(2,0), new(1,1), new(1,2) },
            new Vector2[] { new(0,1), new(1,1), new(2,1), new(2,2) },
            new Vector2[] { new(1,0), new(1,1), new(1,2), new(0,2)},
        };

        protected override Vector2[][] Tiles => _tiles;
        protected override Vector2 StartOffset => new(3, 0);
        public override int Id => 2;
    }
}
