﻿using System.Net.Sockets;

namespace client
{   
    public interface ITCPConnector
    {
        void ConnectAndSendMessage(string server, string message);
    }

    class TCPConnector : ITCPConnector
    {
        public void ConnectAndSendMessage(string server, string message)
        {
            try
            {
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public void ConnectAndSendListOfMessages(string server, List<string> messages)
        {
            try
            {
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                NetworkStream stream = client.GetStream();
                StreamWriter sr = new StreamWriter(stream);
                foreach (string msg in messages) {
                    sr.WriteLine(msg);
                    sr.Flush();
                }

                sr.Close();
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
