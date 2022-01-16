using System.Collections.Generic;
using ConsoleEngine;

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

        public IEnumerable<Vector2> GetLocalTiles()
        {
            foreach (var pos in Tiles[rotationState])
                yield return pos;
        }
        public IEnumerable<Vector2> TilesPosition()
        {
            foreach(var pos in Tiles[rotationState])
            {
                yield return pos + _offset;
            }
        }

        public Vector2 Offset => _offset;

        public int GetWidth()
        {
            int c = 0;
            foreach(var tile in Tiles[rotationState]) { c++; }
            return c;
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
    }
}
