using server;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

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

            var context = new AccessLogContext();

            var insertCount = 0;

            while (true)
            {
                Console.Write("\n\nWaiting for a connection...\n\n");

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!\n");

                NetworkStream stream = client.GetStream();
                StreamReader sr = new StreamReader(stream);

                context.ChangeTracker.AutoDetectChangesEnabled = false;

                var savingLog = false;

                while (sr.Peek() >= 0)
                {
                    var data = sr.ReadLine();

                    if (data != null)
                    {
                        insertCount++;
                        if (insertCount >= 1000)
                        {
                            context.SaveChanges();
                            context = new AccessLogContext();
                            context.ChangeTracker.AutoDetectChangesEnabled = false;
                            insertCount = 0;
                        }

                        savingLog = true;
                        try
                        {
                            AccessLogRegister? receivedRegister = JsonSerializer.Deserialize<AccessLogRegister>(data);

                            if (receivedRegister != null)
                            {
                                context.accessLog.Add(receivedRegister);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: {0}", e);
                        }
                    }
                    else
                    {
                        if (savingLog)
                        {
                            try
                            {
                                context.SaveChanges();
                                Console.WriteLine($"The log was stored in the database succefully!");
                                context.ChangeTracker.AutoDetectChangesEnabled = false;
                                insertCount = 0;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            savingLog = false;
                        }
                    }

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