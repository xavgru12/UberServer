using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UberStrok.Realtime.Server.Comm
{
    internal class SocketCommunicaion
    {
        private static ILog Logging;

        private static void Log(string message)
        {
            try
            {
                Logging.Debug(message);
            }
            catch
            {
            }
        }

        public static void Initialize()
        {
            Logging = LogManager.GetLogger("SocketCommunicaion");
            Log("Starting web socket.");
            new Thread(StartSocket).Start();
            Log("Web Socket Initialised");
        }

        public static void StartSocket()
        {
            try
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5069);
                tcpListener.Start();
                while (true)
                {
                    try
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        NetworkStream stream = tcpClient.GetStream();
                        byte[] array = new byte[tcpClient.ReceiveBufferSize];
                        int count = stream.Read(array, 0, tcpClient.ReceiveBufferSize);
                        string s = SocketOperations.Response(Encoding.UTF8.GetString(array, 0, count).Split(' '));
                        byte[] bytes = Encoding.UTF8.GetBytes(s);
                        stream.Write(bytes, 0, bytes.Length);
                        tcpClient.Close();
                    }
                    catch (Exception ex)
                    {
                        Log("Error:     " + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.ToString());
            }

        }
    }

}
