using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace TCPServerIleriProgramlama
{
    class MainClass
    {
        private static Dictionary<string, string> dlls = new Dictionary<string, string>()
        {
            { "Calculator1.dll", "/Users/omerfarukboran/Projects/Calculator1/Calculator1/obj/Debug/Calculator1.dll" }
        };

        public static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 80);

            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                NetworkStream ns = client.GetStream();
                Console.WriteLine("Connected");

                while (client.Connected)
                {
                    byte[] msg = new byte[1024];
                    ns.Read(msg, 0, msg.Length);
                    string message = Encoding.Default.GetString(msg);
                    string[] messageParts = message.Split('&')[0].Split('#');

                    Console.WriteLine("Server Recieved : " + message);

                    string dllName = messageParts[0];

                    int input;
                    if(!int.TryParse(messageParts[1], out input))
                    {
                        byte[] error = Encoding.ASCII.GetBytes("Input Error!");
                        ns.Write(error, 0, error.Length);
                        break;
                    }

                    if(!dlls.ContainsKey(dllName))
                    {
                        byte[] error = Encoding.ASCII.GetBytes("DLL Name Error!");
                        ns.Write(error, 0, error.Length);
                        break;
                    }

                    string dllPath = dlls[dllName];

                    // DLL'i dinamik olarak yükle
                    Assembly assembly = Assembly.LoadFrom($"{dllPath}");
                    Type type = assembly.GetType($"{dllName.Replace(".dll", "")}.Calculator");
                    object obj = Activator.CreateInstance(type);
                    MethodInfo method = type.GetMethod("Calculate");

                    // Hesaplamayı yap
                    int result = (int)method.Invoke(obj, new object[] { input });

                    // Sonucu gönder
                    string response = $"{dllName}#{input}#{result}&";
                    byte[] buffer = Encoding.ASCII.GetBytes(response);
                    ns.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("Server Sent : " + response);

                    ns.Flush();

                    break;
                }
            }
        }
    }
}
