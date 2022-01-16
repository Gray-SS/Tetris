using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                string[] line = Encoding.UTF8.GetString(result.Buffer).Split();
                string[] arguments = line.Skip(0).ToArray();
                Console.WriteLine(line);

                switch (line[0])
                {
                    case "lb-get":

                        var list = new List<KeyValuePair<string, int>>();
                        string[] data = File.ReadAllLines(filename);

                        foreach(var d in data)
                        {
                            var dataLine = d.Split(';');
                            string username = dataLine[0];
                            int score = Convert.ToInt32(dataLine[1]);

                            list.Add(new KeyValuePair<string, int>(username, score));
                        }

                        string json = JsonConvert.SerializeObject(list);
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        await client.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);

                        break;

                    case "lb-append":
                        var fs = File.OpenWrite(filename);
                        byte[] _buffer = Encoding.UTF8.GetBytes($"{arguments[0]};{arguments[1]}");
                        fs.Write(_buffer, 0, _buffer.Length);
                        break;
                }
            }
        }
    }
}
