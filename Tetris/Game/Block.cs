using System.Collections.Generic;
using Tetris.Framework;

namespace Tetris.Game
{
    public abstract class Block
    {
        protected abstract Vector2[][] Tiles { get; }
        protected abstract Vector2 StartOffset { get; }
        public abstract int Id { get; }

        private Vector2 _offset;
        private int rotationState;

        public Block()
        {
            _offset = StartOffset;
        }

        public IEnumerable<Vector2> TilesPosition()
        {
            foreach(var pos in Tiles[rotationState])
            {
                yield return pos + _offset;
            }
        }

        public void RotateCW()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }
        public void RotateCCW()
        {
            if (rotationState == 0)
                rotationState = Tiles.Length - 1;
            else
                rotationState--;
        }

        public void Move(int x, int y)
        {
            _offset += new Vector2(x, y);
        }

        public void Reset()
        {
            rotationState = 0;
            _offset = StartOffset;
        }

        public void Draw(Graphics g)
        {
            foreach(var tile in TilesPosition())
            {
                g.DrawRectangle(tile.X, tile.Y, 1, 1, null, null, (CColor)Id);
            }
        }
    }
}
