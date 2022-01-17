using ConsoleEngine;
using ConsoleEngine.Graphics;
using ConsoleEngine.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.Game.GameStates
{
    public class OptionsMenuGameState : GameState
    {
        private int _currentSelection;
        private float _inputTimer;

        private string[] _optionsMenuTexts;

        public OptionsMenuGameState(Engine engine) : base(engine) { }

        public override void Load()
        {
            _optionsMenuTexts = new string[]
            {
                "Username",
                "Shadow",
                "Move Left",
                "Move Right",
                "Move Down",
                "Rotate",
                "Restart",
                "Back"
            };
        }

        public override void Update(float dt)
        {
            _inputTimer += dt;

            if (Keyboard.IsKeyDown(StaticValues.Configuration.MenuUpKey) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                if (_currentSelection == 0) _currentSelection = _optionsMenuTexts.Length - 1;
                else _currentSelection -= 1;
            }
            if (Keyboard.IsKeyDown(StaticValues.Configuration.MenuDownKey) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                _currentSelection = (_currentSelection + 1) % _optionsMenuTexts.Length;
            }

            if (Keyboard.IsKeyDown(Keys.Enter) && _inputTimer >= GameOptions.MenuInputDelay)
            {
                _inputTimer = 0f;
                switch (_optionsMenuTexts[_currentSelection])
                {
                    case "Shadow":
                        StaticValues.Configuration.IsShadowEnabled = !StaticValues.Configuration.IsShadowEnabled;
                        StaticValues.Configuration.Update();
                        break;
                    case "Username":
                        string username = string.Empty;
                        Keys key;

                        StaticValues.Configuration.Username = username;

                        do
                        {
                            key = Engine.Wait(GetKeyAsync());

                            if (((int)key >= 'A') && ((int)key <= 'Z'))
                            {
                                username += key.ToString();
                                StaticValues.Configuration.Username = username;
                            }
                            else if (key == Keys.Backspace && username.Length > 0)
                            {
                                username = username.Remove(username.Length - 1, 1);
                                StaticValues.Configuration.Username = username;
                            }
                            else if (key == Keys.Enter)
                            {
                                if (username.Length > 5) username = username.Remove(5, username.Length - 5);
                                StaticValues.Configuration.Username = username;

                                StaticValues.Configuration.Update();
                                Engine.Wait(150);
                                break;
                            }
                        }
                        while (true);
                        break;

                    case "Move Left":
                        StaticValues.Configuration.InGameMoveLeft = Keys.None;
                        StaticValues.Configuration.InGameMoveLeft = Engine.Wait(GetKeyAsync());
                        StaticValues.Configuration.Update();
                        Engine.Wait(50);
                        break;
                    case "Move Right":
                        StaticValues.Configuration.InGameMoveRight = Keys.None;
                        StaticValues.Configuration.InGameMoveRight = Engine.Wait(GetKeyAsync());
                        StaticValues.Configuration.Update();
                        Engine.Wait(50);
                        break;
                    case "Move Down":
                        StaticValues.Configuration.InGameMoveDown = Keys.None;
                        StaticValues.Configuration.InGameMoveDown = Engine.Wait(GetKeyAsync());
                        StaticValues.Configuration.Update();
                        Engine.Wait(50);
                        break;
                    case "Rotate":
                        StaticValues.Configuration.InGameRotation = Keys.None;
                        StaticValues.Configuration.InGameRotation = Engine.Wait(GetKeyAsync());
                        StaticValues.Configuration.Update();
                        Engine.Wait(50);
                        break;
                    case "Restart":
                        StaticValues.Configuration.InGameRestart = Keys.None;
                        StaticValues.Configuration.InGameRestart = Engine.Wait(GetKeyAsync());
                        StaticValues.Configuration.Update();
                        Engine.Wait(50);
                        break;
                    case "Back":
                        GameStateManager.SetCurrentState<MainMenuGameState>();
                        break;
                }
            }
        }

        public override void Draw()
        {
            int first_y = ConsoleHeight / 2 + (int)(_optionsMenuTexts.Length * - 0.5f);
            Graphics.DrawText(ConsoleWidth / 2 - "OPTIONS".Length / 2, first_y - 3, "OPTIONS", CColor.Green, null);

            for (int i = 0; i < _optionsMenuTexts.Length; i++)
            {
                string text = _optionsMenuTexts[i];
                switch (text)
                {
                    case "Shadow": text += $" [{(StaticValues.Configuration.IsShadowEnabled ? "Enabled" : "Disabled")}]"; break;
                    case "Username": text += $" [{StaticValues.Configuration.Username}]"; break;
                    case "Move Left": text += $" [{StaticValues.Configuration.InGameMoveLeft}]"; break;
                    case "Move Right": text += $" [{StaticValues.Configuration.InGameMoveRight}]"; break;
                    case "Move Down": text += $" [{StaticValues.Configuration.InGameMoveDown}]"; break;
                    case "Rotate": text += $" [{StaticValues.Configuration.InGameRotation}]"; break;
                    case "Restart": text += $" [{StaticValues.Configuration.InGameRestart}]"; break;
                }

                int x = ConsoleWidth / 2 - text.Length / 2;
                int y = first_y + i * 2;

                if (text == "Back") y += 1;

                Graphics.DrawText(x, y, text, CColor.White, null);

                if (i == _currentSelection)
                {
                    Graphics.DrawRectangle(x - 2, y, 1, 1, 'c', CColor.Yellow, null);
                }
            }
        }

        public async Task<Keys> GetKeyAsync()
        {
            List<Keys> keys;
            await Task.Delay(150);

            do { }
            while ((keys = Keyboard.GetKeysPressed().ToList()).Count == 0);

            return keys[0];
        }
    }
}
