using ConsoleEngine.Inputs;
using System;

namespace Tetris.Game.Configurations
{
    public class UserConfiguration : BaseConfiguration
    {
        public override string Filename => $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Tetris\\configuration.xml";

        public bool FirstExecute { get; set; }
        public string Username { get; set; }
        public int Id { get; set; }
        public int Highscore { get; set; }

        public Keys InGameRotation { get; set; } = Keys.Up;
        public Keys InGameMoveDown { get; set; } = Keys.Down;
        public Keys InGameMoveLeft { get; set; } = Keys.Left;
        public Keys InGameMoveRight { get; set; } = Keys.Right;
        public Keys InGameRestart { get; set; } = Keys.R;

        public Keys MenuUpKey { get; set; } = Keys.Up;
        public Keys MenuDownKey { get; set; } = Keys.Down;

        public bool IsShadowEnabled { get; set; } = true;

        public UserConfiguration()
        {
            this.FirstExecute = true;
            this.Username = "None";
            this.Highscore = 0;

            Random random = new();
            string id = string.Empty;
            for (int i = 0; i < 5; i++) id += random.Next(10);

            this.Id = Convert.ToInt32(id);
        }
    }
}
