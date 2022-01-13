using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Tetris.Framework;
using Tetris.Game.Blocks;

namespace Tetris.Game
{
    public class TetrisEngine : Engine
    {
        private int _gameState = 0;

        #region State 0
        private int _score = 0;
        private int _highScore = 0;
        private Rectangle _tetrisViewport;
        private GameGrid _gameGrid;
        private Block currentBlock;
        private BlockQueue _blockQueue;
        #endregion

        #region State 1
        private string[] _gameOverMenuTexts;
        private int _currentSelection;
        #endregion

        public TetrisEngine() : base() { }

        public override void Load()
        {
            _gameOverMenuTexts = new string[]
            {
                "Restart",
                "Menu",
                "Exit"
            };

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

            if (_gameState == 0)
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
            else if (_gameState == 1)
            {
                if (Keyboard.IsKeyDown(Keys.Z) && _inputTimer >= _inputDelay * 2f)
                {
                    _inputTimer = 0f;
                    if (_currentSelection == 0) _currentSelection = _gameOverMenuTexts.Length - 1;
                    else _currentSelection -= 1;
                }
                if (Keyboard.IsKeyDown(Keys.S) && _inputTimer >= _inputDelay * 2f)
                {
                    _inputTimer = 0f;
                    _currentSelection = (_currentSelection + 1) % _gameOverMenuTexts.Length;
                }

                if (Keyboard.IsKeyDown(Keys.Enter) && _inputTimer >= _inputDelay * 2f)
                {
                    _inputTimer = 0f;
                    switch (_currentSelection)
                    {
                        case 0:
                            Reset();
                            break;
                        case 1:
                            break;
                        case 2:
                            Process.GetCurrentProcess().Kill();
                            break;
                    }
                }
            }
        }

        public override void Draw()
        {
            Graphics.Clear(CColor.Black);
            Graphics.DrawText(ConsoleWidth - 15, 1, $"FPS: {FPS}", CColor.Red, null);

            if (_gameState == 0)
            {
                Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.DarkGray); //TOP BORDER
                Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Height + 1 + _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.DarkGray); //BOTTOM BORDER 
                Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.DarkGray); //LEFT BORDER
                Graphics.DrawRectangle(_tetrisViewport.Width + 1 + _tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.DarkGray); //RIGHT BORDER

                DrawBlock(currentBlock);
                DrawGameGrid(_gameGrid);

                Graphics.DrawText(_tetrisViewport.Right + 5, 6, $"Score: {_score}", CColor.Yellow, null);
                Graphics.DrawText(_tetrisViewport.Right + 5, 8, "Next Block:", CColor.Magenta, null);

                DrawBlock(_blockQueue.NextBlock, new Vector2(_tetrisViewport.Right + 11 - _blockQueue.NextBlock.GetWidth() / 2, 10));
            }
            else if (_gameState == 1)
            {
                Graphics.DrawText(ConsoleWidth / 2 - "Game Over !".Length / 2, 6, "Game Over !", CColor.Red, null);
                Graphics.DrawText(ConsoleWidth / 2 - $"Highscore: {_highScore}".Length / 2, 8, $"Highscore: {_highScore}", CColor.Green, null);

                for (int i = 0; i < _gameOverMenuTexts.Length; i++)
                {
                    int x = ConsoleWidth / 2 - _gameOverMenuTexts[i].Length / 2;
                    int y = ConsoleHeight / 2 + (_gameOverMenuTexts.Length * i - _gameOverMenuTexts.Length / 2);
                    Graphics.DrawText(x, y, _gameOverMenuTexts[i], CColor.White, null);

                    if (i == _currentSelection)
                    {
                        Graphics.DrawRectangle(x - 2, y, 1, 1, 'c', CColor.Yellow, null);
                    }
                }
            }
        }

        private void Reset()
        {
            _gameState = 0;
            _score = 0;

            _gameGrid.Clear();

            currentBlock = _blockQueue.GetBlock();
            ResetBlock(currentBlock);
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
                _gameState = 1;
                if (_score > _highScore)
                    _highScore = _score;

                _score = 0;
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

        private void DrawBlock(Block block, Vector2 pos)
        {
            foreach(var tile in block.GetLocalTiles())
            {
                Graphics.DrawRectangle(tile.X + pos.X, tile.Y + pos.Y, 1, 1, null, null, (CColor)block.Id);
            }
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