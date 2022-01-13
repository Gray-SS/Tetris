namespace Tetris.Game
{
    public class GameGrid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly int[,] _grid;

        public GameGrid(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            
            _grid = new int[width, height];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
        public bool IsEmpty(int x, int y)
        {
            if (!IsInside(x, y))
                return false;
            return _grid[x, y] == 0;
        }

        public bool IsRowEmpty(int y)
        {
            for (int i = 0; i < Width; i++)
            {
                if (!IsEmpty(i, y))
                    return false;
            }

            return true;
        }
        
        public bool IsRowFull(int y)
        {
            for (int i = 0; i < Width; i++)
            {
                if (_grid[i, y] == 0)
                    return false;
            }

            return true;
        }

        public void ClearRow(int y)
        {
            for (int x = 0; x < Width; x++)
            {
                _grid[x, y] = 0;
            }
        }

        public void MoveRowDown(int y, int count)
        {
            for (int x = 0; x < Width; x++)
            {
                _grid[x, y + count] = _grid[x, y];
                _grid[x, y] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;

            for (int y = Height - 1; y >= 0; y--)
            {
                if (IsRowFull(y))
                {
                    ClearRow(y);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(y, cleared);
                }    
            }

            return cleared;
        }

        public void SetData(int x, int y, int id)
        {
            _grid[x, y] = id;
        }

        public int GetId(int x, int y)
        {
            return _grid[x, y];
        }
    }
}
