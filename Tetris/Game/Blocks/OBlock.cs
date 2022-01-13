using Tetris.Framework;

namespace Tetris.Game.Blocks
{
    public class OBlock : Block
    {
        private readonly Vector2[][] _tiles = new Vector2[][]
        {
            new Vector2[]{ new(0, 0), new(0, 1), new(1, 0), new(1, 1) }
        };

        protected override Vector2[][] Tiles => _tiles;
        protected override Vector2 StartOffset => new(4, 0);
        public override int Id => 4;
    }
}
