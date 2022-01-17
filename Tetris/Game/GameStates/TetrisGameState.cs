using ConsoleEngine;
using ConsoleEngine.Graphics;
using ConsoleEngine.Inputs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.Game.GameStates
{
    public class TetrisGameState : GameState
    {
        private int _score = 0;
        private bool _gameOver;
        private float _delay = 0f;
        private Rectangle _tetrisViewport;
        private GameGrid _gameGrid;
        private Block currentBlock;
        private BlockQueue _blockQueue;

        private int _tick;

        private float _timer;
        private float _delaySubTimer;
        private float _inputTimer;

        public TetrisGameState(Engine engine) : base(engine) { }

        public override void OnStateCalled()
        {
            StartGame();
        }

        public override void Load()
        {
            _delay = GameOptions.InGameBlockFallStartSpeedDelay;
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

        public override void Update(float dt)
        {
            _delaySubTimer += dt;
            _inputTimer += dt;
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

                MoveDown(currentBlock);
            }

            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameRestart) && _inputTimer >= GameOptions.InGameInputDelay * 5f) GameOver();
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveRight) && _inputTimer >= GameOptions.InGameInputDelay) { _inputTimer = 0f; MoveRight(currentBlock); }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveLeft) && _inputTimer >= GameOptions.InGameInputDelay) { _inputTimer = 0f; MoveLeft(currentBlock); }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveDown) && _inputTimer >= GameOptions.InGameInputDelay * 0.35f)
            {
                _inputTimer = 0f;
                _score += 1;
                MoveDown(currentBlock);
            }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameRotation) && _inputTimer >= GameOptions.InGameInputDelay * 1.5f) { _inputTimer = 0f; Rotate(currentBlock); }
        }

        public override void Draw()
        {
            Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.White); //TOP BORDER
            Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Height + 1 + _tetrisViewport.Y, _tetrisViewport.Width + 2, 1, null, null, CColor.White); //BOTTOM BORDER 
            Graphics.DrawRectangle(_tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.White); //LEFT BORDER
            Graphics.DrawRectangle(_tetrisViewport.Width + 1 + _tetrisViewport.X, _tetrisViewport.Y, 1, _tetrisViewport.Height + 2, null, null, CColor.White); //RIGHT BORDER

            if (StaticValues.Configuration.IsShadowEnabled && !_gameOver) DrawShadow(currentBlock);
            DrawBlock(currentBlock);
            DrawGameGrid(_gameGrid);

            Graphics.DrawText(_tetrisViewport.Right + 5, 6, $"Score: {_score}", CColor.Yellow, null);
            Graphics.DrawText(_tetrisViewport.Right + 5, 8, "Next Block:", CColor.Magenta, null);
            Graphics.DrawText(ConsoleWidth - 15, 2, $"Delay: {_delay}", CColor.Red, null);

            DrawBlock(_blockQueue.NextBlock, new Vector2(_tetrisViewport.Right + 11 - _blockQueue.NextBlock.GetWidth() / 2, 10), (CColor)_blockQueue.NextBlock.Id);
        }

        private void StartGame()
        {
            _delaySubTimer = 0f;
            _timer = 0f;
            _inputTimer = 0f;

            _score = 0;
            _delay = GameOptions.InGameBlockFallStartSpeedDelay;

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
            foreach (var tile in block.TilesPosition())
            {
                _gameGrid.SetData(tile.X - _tetrisViewport.X - 1, tile.Y - _tetrisViewport.Y - 1, block.Id);
            }

            _score += 10;

            int c = _gameGrid.ClearFullRows();
            int bonusRowCleared = (int)(100 * c + 100 * ((c - 1) * 0.25f + 0.25f));
            if (bonusRowCleared > 0) _score += bonusRowCleared;

            if (!_gameGrid.IsRowEmpty(1) && !_gameGrid.IsRowEmpty(2))
            {
                GameOver();
            }
        }

        private void GameOver()
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

        private void DrawShadow(Block block)
        {
            Vector2 pos = new(block.Offset.X, block.Offset.Y);
            while (BlockFits(block, pos))
                pos.Y += 1;
            pos.Y -= 1;

            DrawBlock(block, pos, CColor.DarkGray);
        }

        private void DrawBlock(Block block)
        {
            foreach (var tile in block.TilesPosition())
            {
                Graphics.DrawRectangle(tile.X, tile.Y, 1, 1, null, null, (CColor)block.Id);
            }
        }
        private void DrawBlock(Block block, Vector2 pos, CColor color)
        {
            foreach (var tile in block.GetLocalTiles())
            {
                Graphics.DrawRectangle(tile.X + pos.X, tile.Y + pos.Y, 1, 1, null, null, color);
            }
        }

        private void DrawGameGrid(GameGrid gameGrid)
        {
            int x = _tetrisViewport.X, y = _tetrisViewport.Y;
            for (int j = 0; j < gameGrid.Height; j++)
            {
                for (int i = 0; i < gameGrid.Width; i++)
                {
                    int id = gameGrid.GetId(i, j);
                    if (id != 0)
                    {
                        Graphics.DrawRectangle(x + i + 1, y + j + 1, 1, 1, null, null, (CColor)id);
                    }
                }
            }
        }
    }
}
