using System.Net;
using System.Net.Sockets;
class MyTcpListener
{
    public static void Main()
    {
        TcpListener server = null;
        try
        {
            var port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);

            server.Start();

            var bytes = new Byte[256];
            String data = null;

            while (true)
            {
                Console.Write("\n\nWaiting for a connection...\n\n");

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!\n");

                data = null;

                NetworkStream stream = client.GetStream();

                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }
    }
}