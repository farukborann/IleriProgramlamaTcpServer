using System;
using System.Net.Sockets;
using System.Text:

namespace TCPClientIleriProgramlama
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 80);

            string message = "Calculator1.dll#17&";

            byte[] data = Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            Console.WriteLine("Client Sent: {0}", message);

            data = new byte[1024];
            stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            Console.WriteLine("Client Received: {0}", responseData);
        }
    }
}
