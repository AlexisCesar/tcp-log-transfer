using System.Net.Sockets;

namespace client
{
    class TCPConnector
    {
        public void ConnectAndSendListOfMessages(string server, List<string> messages)
        {
            try
            {
                int port = 13000;
                var client = new TcpClient(server, port);

                var stream = client.GetStream();
                var streamWriter = new StreamWriter(stream);

                foreach (string msg in messages) {
                    streamWriter.WriteLine(msg);
                    streamWriter.Flush();
                }

                streamWriter.Close();
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
