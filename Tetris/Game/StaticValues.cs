using System.Net.Sockets;
using Tetris.Game.Configurations;

namespace Tetris.Game
{
    public static class StaticValues
    {
        public static int Score { get; set; }
        public static UdpClient UdpClient { get; private set; }
        public static UserConfiguration Configuration { get; private set; }

        public static void Initialize()
        {
            UdpClient = new UdpClient();
            Configuration = BaseConfiguration.Load<UserConfiguration>();
        }
    }
}
