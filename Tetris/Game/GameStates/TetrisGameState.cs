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

        private float _inputTimer;

        private TetrisGame _game = new TetrisGame(new Rectangle(2, 2, 10, 22));

        public TetrisGameState(Engine engine) : base(engine) { }

        public override void OnStateCalled()
        {
            StartGame();
        }

        public override void Load()
        {

            _game.Load();
        }

        public override void Update(float dt)
        {
            _inputTimer += dt;

            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGamePause) && _inputTimer >= GameOptions.InGameInputDelay)
            {
                _inputTimer = 0f;
                _game.PauseResume();
            }

            if (_game.Paused)
                return;

            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameRestart) && _inputTimer >= GameOptions.InGameInputDelay * 5f) _game.EndGame();
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveRight) && _inputTimer >= GameOptions.InGameInputDelay) { _inputTimer = 0f; _game.MoveRightCurrentBlock(); }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveLeft) && _inputTimer >= GameOptions.InGameInputDelay) { _inputTimer = 0f; _game.MoveLeftCurrentBlock(); }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveDown) && _inputTimer >= GameOptions.InGameInputDelay * 0.35f)
            {
                _inputTimer = 0f;
                _game.Score += 1;
                _game.MoveDownCurrentBlock();
            }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameRotation) && _inputTimer >= GameOptions.InGameInputDelay * 1.5f) { _inputTimer = 0f; _game.Rotate(); }

            _game.Update(dt);
        }

        public override void Draw()
        {
            _game.Draw(Graphics);
        }

       private void StartGame()
        {
            _inputTimer = 0f;
            _game.StartGame();
        }
    }
}
