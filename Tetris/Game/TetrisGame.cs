using ConsoleEngine;
using ConsoleEngine.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tetris.Game.GameStates;

namespace Tetris.Game
{
    public class TetrisGame
    {
        public int Score
        {
            get => _score;
            set => _score = value;
        }
        public bool Paused => _paused;

        private int _score;
        private bool _paused;

        private float _delay = 0f;

        private int _tick;

        private float _timer;
        private float _delaySubTimer;

        private bool _gameOver;
        private Rectangle _tetrisViewport;
        private Block currentBlock;
        private BlockQueue _blockQueue;
        private GameGrid _gameGrid;

        public TetrisGame(Rectangle tetrisViewport)
        {
            _tetrisViewport = tetrisViewport;
        }

        public void Load()
        {
            _delay = GameOptions.InGameBlockFallStartSpeedDelay;

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

        public void Update(float dt)
        {
            if (_paused)
                return;

            _delaySubTimer += dt;
            _timer += dt;

            if (_delaySubTimer >= GameOptions.InGameBlockFallSpeedIncrementDelay)
            {
                _tick += 1;
                _delay = GameOptions.InGameBlockFallStartSpeedDelay * MathF.Pow(GameOptions.InGameBlockFallSpeedModifier, _tick);
                _delaySubTimer = 0f;
            }

            if (_timer >= _delay)
            {
                _timer = 0f;

                MoveDownCurrentBlock();
            }
        }

        public void Draw(GraphicsDevice graphics)
        {
            graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.White); //TOP BORDER
            graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Height + 1 + _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.White); //BOTTOM BORDER 
            graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.White); //LEFT BORDER
            graphics.DrawRectangle(_tetrisViewport.Width + 1 + _tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.White); //RIGHT BORDER

            if (StaticValues.Configuration.IsShadowEnabled && !_gameOver) DrawShadow(currentBlock, graphics);
            DrawBlock(currentBlock, graphics);
            DrawGameGrid(_gameGrid, graphics);

            graphics.DrawText(_tetrisViewport.Right + 5, 6, $"Score: {_score}", CColor.Yellow, null);
            graphics.DrawText(_tetrisViewport.Right + 5, 8, "Next Block:", CColor.Magenta, null);
            if (_paused) graphics.DrawText(_tetrisViewport.Right + 20, 6, "GAME PAUSED", CColor.Cyan, null);

            DrawBlock(_blockQueue.NextBlock, new Vector2(_tetrisViewport.Right + 11 - _blockQueue.NextBlock.GetWidth() / 2, 10), (CColor)_blockQueue.NextBlock.Id, graphics);
        }

        private void ResetBlock(Block block)
        {
            block.Reset();
            block.Move(_tetrisViewport.X + 1, _tetrisViewport.Y + 1);
        }

        private void PlaceBlock(Block block)
        {
            foreach (var tile in block.TilesPosition())
            {
                _gameGrid.SetData(tile.X - _tetrisViewport.X - 1, tile.Y - _tetrisViewport.Y - 1, block.Id);
            }

            _score += 10;
            Task.Run(() => StaticValues.BlockPlacedSound.Play(false));

            int c = _gameGrid.ClearFullRows();
            if (c > 0) Task.Run(() => StaticValues.ExplosionSound.Play(false));
            int bonusRowCleared = (int)(100 * c + 100 * ((c - 1) * 0.25f + 0.25f));
            if (bonusRowCleared > 0) _score += bonusRowCleared;

            if (!_gameGrid.IsRowEmpty(1) && !_gameGrid.IsRowEmpty(2))
            {
                EndGame();
            }
        }

        public void EndGame()
        {
            _gameOver = true;

            Task.Run(() => SoundMixer.PlayGameOverSound());

            for (int j = _gameGrid.Height - 1; j >= 0; j--)
            {
                for (int i = 0; i < _gameGrid.Width; i++)
                {
                    _gameGrid.SetData(i, j, (int)CColor.White);
                }
                Engine.Wait(50);
            }
            Engine.Wait(100);

            StaticValues.Score = _score;

            if (_score > StaticValues.Configuration.Highscore)
            {
                StaticValues.Configuration.Highscore = _score;
                StaticValues.Configuration.Update();
            }

            GameStateManager.SetCurrentState<GameOverGameState>();
            _gameOver = false;
        }

        public void StartGame()
        {
            _score = 0;
            _tick = 0;
            _gameGrid.Clear();

            _delay = GameOptions.InGameBlockFallStartSpeedDelay;
            _delaySubTimer = 0f;
            _timer = 0f;

            currentBlock = _blockQueue.GetBlock();
            ResetBlock(currentBlock);
        }

        public void PauseResume()
        {
            _paused = !_paused;
        }

        public void Rotate()
        {
            currentBlock.RotateCW();

            if (!BlockFits(currentBlock))
                currentBlock.RotateCCW();
        }

        public void TeleportToShadow()
        {
            var position = GetShadowPos(currentBlock);
            currentBlock.Move(position.X - currentBlock.Offset.X, position.Y - currentBlock.Offset.Y);
        }

        public void MoveDownCurrentBlock()
        {
            currentBlock.Move(0, 1);

            if (!BlockFits(currentBlock))
            {
                currentBlock.Move(0, -1);

                PlaceBlock(currentBlock);

                currentBlock = _blockQueue.GetBlock();
                ResetBlock(currentBlock);
            }
        }

        public void MoveRightCurrentBlock()
        {
            currentBlock.Move(1, 0);

            if (!BlockFits(currentBlock))
                currentBlock.Move(-1, 0);
        }

        public void MoveLeftCurrentBlock()
        {
            currentBlock.Move(-1, 0);

            if (!BlockFits(currentBlock))
                currentBlock.Move(1, 0);
        }

        private bool BlockFits(Block block)
        {
            foreach (var tile in block.TilesPosition())
            {
                if (!_gameGrid.IsEmpty(tile.X - _tetrisViewport.X - 1, tile.Y - _tetrisViewport.Y - 1))
                    return false;
            }
            return true;
        }

        private bool BlockFits(Block block, Vector2 pos)
        {
            foreach (var tile in block.GetLocalTiles())
            {
                if (!_gameGrid.IsEmpty(tile.X + pos.X - _tetrisViewport.X - 1, tile.Y - _tetrisViewport.Y + pos.Y - 1))
                    return false;
            }
            return true;
        }

        private Vector2 GetShadowPos(Block block)
        {
            Vector2 pos = new(block.Offset.X, block.Offset.Y);
            while (BlockFits(block, pos))
                pos.Y += 1;
            pos.Y -= 1;
            return pos;
        }

        private void DrawShadow(Block block, GraphicsDevice graphics)
        {
            DrawBlock(block, GetShadowPos(block), CColor.DarkGray, graphics);
        }

        private void DrawBlock(Block block, GraphicsDevice graphics)
        {
            foreach (var tile in block.TilesPosition())
            {
                graphics.DrawRectangle(tile.X, tile.Y, 1, 1, null, null, (CColor)block.Id);
            }
        }
        private void DrawBlock(Block block, Vector2 pos, CColor color, GraphicsDevice graphics)
        {
            foreach (var tile in block.GetLocalTiles())
            {
                graphics.DrawRectangle(tile.X + pos.X, tile.Y + pos.Y, 1, 1, null, null, color);
            }
        }

        private void DrawGameGrid(GameGrid gameGrid, GraphicsDevice graphics)
        {
            int x = _tetrisViewport.X, y = _tetrisViewport.Y;
            for (int j = 0; j < gameGrid.Height; j++)
            {
                for (int i = 0; i < gameGrid.Width; i++)
                {
                    int id = gameGrid.GetId(i, j);
                    if (id != 0)
                    {
                        graphics.DrawRectangle(x + i + 1, y + j + 1, 1, 1, null, null, (CColor)id);
                    }
                }
            }
        }
    }
}
