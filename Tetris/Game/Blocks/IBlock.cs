using Tetris.Framework;

namespace Tetris.Game.Blocks
{
    public class IBlock : Block
    {
        private readonly Vector2[][] _tiles = new Vector2[][] {
            new Vector2[] { new(0, 1), new(1, 1), new(2, 1), new(3, 1) },
            new Vector2[] { new(2, 0), new(2, 1), new(2, 2), new(2, 3) },
            new Vector2[] { new(0, 2), new(1, 2), new(2, 2), new(3, 2) },
            new Vector2[] { new(1, 0), new(1, 1), new(1, 2), new(1, 3) },
        };

        protected override Vector2[][] Tiles => _tiles;
        protected override Vector2 StartOffset => new (3, -1);
        public override int Id => 1;
    }
}
