using System;
using System.Threading;
using Tetris.Framework;
using Tetris.Game.Blocks;

namespace Tetris.Game
{
    public class TetrisEngine : Engine
    {
        private int _score = 0;
        private int _highScore = 0;
        private Rectangle _tetrisViewport;
        private GameGrid _gameGrid;
        private Block currentBlock;
        private BlockQueue _blockQueue;
        private bool _gameOver = false;

        public TetrisEngine() : base() { }

        public override void Load()
        {
            _tetrisViewport = new Rectangle(4, ConsoleHeight / 2 - 22 / 2, 10, 22);
            _gameGrid = new GameGrid(_tetrisViewport.Width, _tetrisViewport.Height);
            _gameGrid.OnRowCleared += (y) =>
            {
                for (int i = 0; i < _gameGrid.Width; i++)
                {
                    int temp = _gameGrid.GetId(i, y);
                    _gameGrid.SetData(i, y, (int)CColor.White);
                    Thread.Sleep(5);
                }
            };

            _blockQueue = new BlockQueue();

            currentBlock = _blockQueue.GetBlock();
            ResetBlock(currentBlock);
        }

        private float _timer = 0f;
        private float _inputTimer = 0f;

        private float _inputDelay = 0.1f;
        public override void Update(float dt)
        {
            _inputTimer += dt;

            if (!_gameOver)
            {
                _timer += dt;
                if (_timer >= 0.5f)
                {
                    _timer = 0f;

                    MoveDown(currentBlock);
                }

                if (Keyboard.IsKeyDown(Keys.D) && _inputTimer >= _inputDelay) { _inputTimer = 0f; MoveRight(currentBlock); }
                if (Keyboard.IsKeyDown(Keys.Q) && _inputTimer >= _inputDelay) { _inputTimer = 0f; MoveLeft(currentBlock); }
                if (Keyboard.IsKeyDown(Keys.S) && _inputTimer >= _inputDelay) { _inputTimer = 0f; MoveDown(currentBlock); }
                if (Keyboard.IsKeyDown(Keys.R) && _inputTimer >= _inputDelay * 1.5f) { _inputTimer = 0f; Rotate(currentBlock); }
            }
        }

        public override void Draw()
        {
            Graphics.Clear(CColor.Black);

            Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.DarkGray); //TOP BORDER
            Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Height + 1 + _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.DarkGray); //BOTTOM BORDER 
            Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.DarkGray); //LEFT BORDER
            Graphics.DrawRectangle(_tetrisViewport.Width + 1 + _tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.DarkGray); //RIGHT BORDER

            DrawBlock(currentBlock);
            DrawGameGrid(_gameGrid);

            Graphics.DrawText(ConsoleWidth - 20, 1, $"FPS: {FPS}", CColor.Red, null);
            Graphics.DrawText(ConsoleWidth - 20, 2, $"Console Width: {ConsoleWidth}", CColor.Green, null);
            Graphics.DrawText(ConsoleWidth - 20, 3, $"Console Height: {ConsoleHeight}", CColor.Blue, null);
            Graphics.DrawText(_tetrisViewport.Right + 5, 4, $"Score: {_score}", CColor.Yellow, null);
            if (_gameOver) Graphics.DrawText(_tetrisViewport.Right + 6, 5, $"Game Over !", CColor.Red, null);
        }

        private void Rotate(Block block)
        {
            block.RotateCW();

            if (!BlockFits(block))
                block.RotateCCW();
        }

        private void MoveDown(Block block)
        {
            block.Move(0, 1);

            if (!BlockFits(block))
            {
                block.Move(0, -1);

                PlaceBlock(block);

                currentBlock = _blockQueue.GetBlock();
                ResetBlock(currentBlock);
            }
        }

        private void MoveRight(Block block)
        {
            block.Move(1, 0);

            if (!BlockFits(block))
                block.Move(-1, 0);
        }

        private void MoveLeft(Block block)
        {
            block.Move(-1, 0);

            if (!BlockFits(block))
                block.Move(1, 0);
        }

        private void ResetBlock(Block block)
        {
            block.Reset();
            block.Move(_tetrisViewport.X + 1, _tetrisViewport.Y + 1);
        }

        private void PlaceBlock(Block block)
        {
            foreach(var tile in block.TilesPosition())
            {
                _gameGrid.SetData(tile.X - _tetrisViewport.X - 1, tile.Y - _tetrisViewport.Y - 1, block.Id);
            }

            if (!_gameGrid.IsRowEmpty(1) && !_gameGrid.IsRowEmpty(2))
            {
                _gameOver = true;
                if (_score > _highScore)
                    _highScore = _score;
            }

            _score += 10;

            _score += 100 * _gameGrid.ClearFullRows();
        }

        private bool BlockFits(Block block)
        {
            foreach(var tile in block.TilesPosition())
            {
                if (!_gameGrid.IsEmpty(tile.X - _tetrisViewport.X - 1, tile.Y - _tetrisViewport.Y - 1))
                    return false;
            }
            return true;
        }

        private void DrawBlock(Block block)
        {
            foreach (var tile in block.TilesPosition())
            {
                Graphics.DrawRectangle(tile.X, tile.Y, 1, 1, null, null, (CColor)block.Id);
            }
        }

        private void DrawGameGrid(GameGrid gameGrid)
        {
            int x = _tetrisViewport.X, y = _tetrisViewport.Y;
            for(int j = 0; j < gameGrid.Height; j++)
            {
                for (int i = 0; i < gameGrid.Width; i++)
                {
                    int id = gameGrid.GetId(i, j);
                    if (id != 0)
                    {
                        Graphics.DrawRectangle(x + i * 1 + 1, y + j * 1 + 1, 1, 1, null, null, (CColor)id);
                    }
                }
            }
        }
    }
}