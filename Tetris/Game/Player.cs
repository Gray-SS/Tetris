using Newtonsoft.Json;

namespace Tetris.Game
{
    public class Player
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("highscore")]
        public int Highscore { get; set; }
    }
}
