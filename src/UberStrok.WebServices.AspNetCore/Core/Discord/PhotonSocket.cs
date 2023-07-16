using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UberStrok.WebServices.AspNetCore.Core.Discord
{
    internal class PhotonSocket
    {
        public static IPAddress localIP
        {
            get
            {
                try
                {
                    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress[] addressList = host.AddressList;
                    foreach (IPAddress ip in addressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip;
                        }
                    }
                }
                catch
                {
                    return null;
                }
                return null;
            }
        }

        public static string ExecuteClientSocket(string message)
        {
            try
            {
                TcpClient client = new TcpClient(Startup.WebServiceConfiguration.ServiceSocket, 5069);
                NetworkStream nwStream = client.GetStream();
                byte[] send = Encoding.UTF8.GetBytes(message);
                nwStream.Write(send, 0, send.Length);
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                string ret = Encoding.UTF8.GetString(bytesToRead, 0, bytesRead);
                client.Close();
                return ret;
            }
            catch (ArgumentNullException e3)
            {
                Console.WriteLine(Environment.NewLine + "NullException : " + e3.ToString() + Environment.NewLine);
                return e3.ToString();
            }
            catch (SocketException e2)
            {
                Console.WriteLine(Environment.NewLine + "SocketException : " + e2.ToString() + Environment.NewLine);
                return e2.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(Environment.NewLine + "Unexpected exception : " + e.ToString() + Environment.NewLine);
                return e.ToString();
            }
        }
    }
}
