using ConsoleEngine;
using ConsoleEngine.Graphics;
using ConsoleEngine.Inputs;
using System.Diagnostics;

namespace Tetris.Game.GameStates
{
    public class GameOverGameState : GameState
    {
        private int _currentSelection;
        private float _inputTimer;
        private string[] _gameOverMenuTexts;

        public GameOverGameState(Engine engine) : base(engine) { }

        public override void Load()
        {
            _gameOverMenuTexts = new string[]
            {
                "Restart",
                "Menu",
                "Exit"
            };
        }

        public override void Update(float dt)
        {
            _inputTimer += dt;

            if (Keyboard.IsKeyDown(StaticValues.Configuration.MenuUpKey) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                if (_currentSelection == 0) _currentSelection = _gameOverMenuTexts.Length - 1;
                else _currentSelection -= 1;
            }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.MenuDownKey) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                _currentSelection = (_currentSelection + 1) % _gameOverMenuTexts.Length;
            }

            if (Keyboard.IsKeyDown(Keys.Enter) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                switch (_currentSelection)
                {
                    case 0:
                        GameStateManager.SetCurrentState<TetrisGameState>();
                        break;
                    case 1:
                        GameStateManager.SetCurrentState<MainMenuGameState>();
                        break;
                    case 2:
                        Process.GetCurrentProcess().Kill();
                        break;
                }
            }
        }

        public override void Draw()
        {
            int first_y = ConsoleHeight / 2 + (int)(_gameOverMenuTexts.Length * -0.5f);
            Graphics.DrawText(ConsoleWidth / 2 - "Game Over !".Length / 2, first_y - 4, "Game Over !", CColor.Red, null);
            Graphics.DrawText(ConsoleWidth / 2 - $"Score: {StaticValues.Score}".Length / 2, first_y - 8, $"Score: {StaticValues.Score}", CColor.Green, null);
            Graphics.DrawText(ConsoleWidth / 2 - $"Highscore: {StaticValues.Configuration.Highscore}".Length / 2, first_y - 6, $"Highscore: {StaticValues.Configuration.Highscore}", CColor.Green, null);


            for (int i = 0; i < _gameOverMenuTexts.Length; i++)
            {
                int x = ConsoleWidth / 2 - _gameOverMenuTexts[i].Length / 2;
                int y = first_y + i * 2;
                Graphics.DrawText(x, y, _gameOverMenuTexts[i], CColor.White, null);

                if (i == _currentSelection)
                {
                    Graphics.DrawRectangle(x - 2, y, 1, 1, 'c', CColor.Yellow, null);
                }
            }
        }
    }
}
