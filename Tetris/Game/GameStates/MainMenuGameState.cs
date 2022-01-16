using ConsoleEngine;
using ConsoleEngine.Graphics;
using ConsoleEngine.Inputs;
using System.Diagnostics;

namespace Tetris.Game.GameStates
{
    public class MainMenuGameState : GameState
    {
        private string[] _gameMenuTexts;

        private int _currentSelection;
        private float _inputTimer;

        public MainMenuGameState(Engine engine) : base(engine) { }

        public override void Load()
        {
            _gameMenuTexts = new string[]
            {
                "Start",
                "Options",
                "Leaderboard",
                "Exit"
            };
        }

        public override void Update(float dt)
        {
            _inputTimer += dt;

            if (Keyboard.IsKeyDown(StaticValues.Configuration.MenuUpKey) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                if (_currentSelection == 0) _currentSelection = _gameMenuTexts.Length - 1;
                else _currentSelection -= 1;
            }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.MenuDownKey) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                _currentSelection = (_currentSelection + 1) % _gameMenuTexts.Length;
            }

            if (Keyboard.IsKeyDown(Keys.Enter) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                switch (_gameMenuTexts[_currentSelection])
                {
                    case "Start":
                        GameStateManager.SetCurrentState<TetrisGameState>();
                        break;
                    case "Leaderboard":
                        GameStateManager.SetCurrentState<LeaderboardGameState>();
                        break;
                    case "Exit":
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "Options":
                        GameStateManager.SetCurrentState<OptionsMenuGameState>();
                        break;
                }
            }
        }

        public override void Draw()
        {
            int first_y = ConsoleHeight / 2 + (int)(_gameMenuTexts.Length * -0.5f);
            Graphics.DrawText(ConsoleWidth / 2 - "TETRIS".Length / 2, first_y - 3, "TETRIS", CColor.Magenta, null);

            for (int i = 0; i < _gameMenuTexts.Length; i++)
            {
                int x = ConsoleWidth / 2 - _gameMenuTexts[i].Length / 2;
                int y = first_y + i * 2;
                Graphics.DrawText(x, y, _gameMenuTexts[i], CColor.White, null);

                if (i == _currentSelection)
                {
                    Graphics.DrawRectangle(x - 2, y, 1, 1, 'c', CColor.Yellow, null);
                }
            }
        }
    }
}
