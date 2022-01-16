using ConsoleEngine;

namespace Tetris.Game.Blocks
{
    public class TBlock : Block
    {
        private readonly Vector2[][] _tiles = new Vector2[][]
        {   
            new Vector2[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1) },
            new Vector2[] { new(1, 0), new(1, 1), new(1, 2), new(2, 1) },
            new Vector2[] { new(0, 1), new(1, 1), new(2, 1), new(1, 2) },
            new Vector2[] { new(0, 1), new(1, 1), new(1, 0), new(1, 2) }
        };

        protected override Vector2[][] Tiles => _tiles;
        public override int Id => 6;
        protected override Vector2 StartOffset => new(3, 0);
    }
}
