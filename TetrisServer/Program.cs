using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TetrisServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string filename = "lb.csv";

            if (!File.Exists(filename)) File.Create(filename);

            UdpClient client = new(new IPEndPoint(IPAddress.Any, 9981));

            while (true)
            {
                var result = await client.ReceiveAsync();
                string cmd = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine(cmd);

                switch (cmd)
                {
                    case "lb-get":

                        var list = new List<KeyValuePair<string, int>>();
                        string[] data = File.ReadAllLines(filename);

                        foreach(var d in data)
                        {
                            var line = d.Split(';');
                            string username = line[0];
                            int score = Convert.ToInt32(line[1]);

                            list.Add(new KeyValuePair<string, int>(username, score));
                        }

                        string json = JsonConvert.SerializeObject(list);
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        await client.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);

                        break;
                }
            }
        }
    }
}
