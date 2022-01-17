using ConsoleEngine;
using ConsoleEngine.Graphics;
using ConsoleEngine.Inputs;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Game.GameStates
{
    public class LeaderboardGameState : GameState
    {
        private List<Player> _leaderboards = new();

        public LeaderboardGameState(Engine engine) : base(engine) { }

        public async override void OnStateCalled()
        {
            /*
            _leaderboards = new List<KeyValuePair<string, int>>()
            {
                new("Sasha", 1150),
                new("Maxime", 3756),
                new("Andy", 9741),
                new("Snico", 350),
                new("Snico", 350),
                new("Snico", 350),
                new("Snico", 350),
                new("Snico", 350),
                new("Snico", 350),
                new("Snico", 350),
            };
            */

            _leaderboards = await StaticValues.GetLeaderboards();
        }

        public override void Update(float dt)
        {
            if (Keyboard.IsKeyDown(Keys.Escape)) GameStateManager.SetCurrentState<MainMenuGameState>();
        }

        public override void Draw()
        {
            if (_leaderboards == null)
                return;

            int start_y = 6;

            Graphics.DrawText(ConsoleWidth / 2 - "LEADERBOARD".Length / 2, start_y - 3, "LEADERBOARD", CColor.Cyan, null);

            int i = 0;
            foreach(var values in _leaderboards.OrderByDescending(x => x.Highscore))
            {
                var rankColor = i switch
                {
                    0 => CColor.DarkYellow,
                    1 => CColor.DarkGray,
                    2 => CColor.DarkRed,
                    _ => CColor.White,
                };
                Graphics.DrawText(5, start_y + 2 * i, (i + 1).ToString(), rankColor, null);
                Graphics.DrawText(8, start_y + 2 * i, values.Username, CColor.White, null);
                Graphics.DrawText(ConsoleWidth - 8 - _leaderboards.Max(x => x.Highscore).ToString().Length, start_y + 2 * i, values.Highscore.ToString(), CColor.White, null);
                i++;
            }

            Graphics.DrawText(ConsoleWidth / 2 - "[PRESS ESC TO RETURN TO THE MENU]".Length / 2, ConsoleHeight - 3, "[PRESS ESC TO RETURN TO THE MENU]", CColor.DarkGray, null);
        }
    }
}
