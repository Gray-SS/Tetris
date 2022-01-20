using ConsoleEngine;
using ConsoleEngine.Inputs;

namespace Tetris.Game.GameStates
{
    public class VersusTetrisGameState : GameState
    {
        private float _inputTimer;

        private TetrisGame _player1Game = new TetrisGame(new Rectangle(2, 2, 10, 22));
        private TetrisGame _player2Game = new TetrisGame(new Rectangle(2, 2, 10, 22));

        public VersusTetrisGameState(Engine engine) : base(engine) { }

        public override void OnStateCalled()
        {
            StartGame();
        }

        public override void Initialize()
        {
            TetrisGame _player1Game = new TetrisGame(new Rectangle(2, 2, 10, 22));
            TetrisGame _player2Game = new TetrisGame(new Rectangle(ConsoleWidth - 14, 2, 10, 22));
        }

        public override void Load()
        {
            _player1Game.Load();
            _player2Game.Load();
        }

        public override void Update(float dt)
        {
            _inputTimer += dt;

            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameRestart) && _inputTimer >= GameOptions.InGameInputDelay * 5f) _player2Game.EndGame();
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveRight) && _inputTimer >= GameOptions.InGameInputDelay) { _inputTimer = 0f; _player2Game.MoveRightCurrentBlock(); }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveLeft) && _inputTimer >= GameOptions.InGameInputDelay) { _inputTimer = 0f; _player2Game.MoveLeftCurrentBlock(); }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameMoveDown) && _inputTimer >= GameOptions.InGameInputDelay * 0.35f)
            {
                _inputTimer = 0f;
                _player2Game.Score += 1;
                _player2Game.MoveDownCurrentBlock();
            }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.InGameRotation) && _inputTimer >= GameOptions.InGameInputDelay * 1.5f) { _inputTimer = 0f; _player2Game.Rotate(); }

            _player1Game.Update(dt);
            _player2Game.Update(dt);
        }

        public override void Draw()
        {
            _player1Game.Draw(Graphics);
            _player2Game.Draw(Graphics);
        }

        private void StartGame()
        {
            _inputTimer = 0f;
            _player1Game.StartGame();
            _player2Game.StartGame();
        }
    }
}
