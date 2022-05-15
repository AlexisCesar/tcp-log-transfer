using Microsoft.Data.SqlClient;
using server;
using System.Data;
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


            var tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("Id", typeof(Int32)));
            tbl.Columns.Add(new DataColumn("SourceIPAddress", typeof(string)));
            tbl.Columns.Add(new DataColumn("DestinationIPAddress", typeof(string)));
            tbl.Columns.Add(new DataColumn("BrazilianTime", typeof(DateTime)));
            tbl.Columns.Add(new DataColumn("Url", typeof(string)));
            tbl.Columns.Add(new DataColumn("StatusCode", typeof(Int32)));
            tbl.Columns.Add(new DataColumn("RequestBytes", typeof(Int32)));

            while (true)
            {
                Console.Write("\n\nWaiting for a connection...\n\n");

                TcpClient client = server.AcceptTcpClient();
                new Thread(() =>
                {
                    using (client)
                    {
                        Console.WriteLine("Connected!!!");

                        NetworkStream stream = client.GetStream();
                        StreamReader sr = new StreamReader(stream);

                        var commitCount = 0;

                        while (sr.Peek() >= 0)
                        {
                            var data = sr.ReadLine();

                            if (data != null)
                            {
                                commitCount++;
                                AccessLogRegister? receivedRegister;

                                try
                                {
                                    receivedRegister = JsonSerializer.Deserialize<AccessLogRegister>(data);
                                } catch (Exception e)
                                {
                                    Console.WriteLine("Wrong JSON format received.");
                                    return;
                                }
                                

                                if (receivedRegister != null)
                                {
                                    DataRow dr = tbl.NewRow();
                                    dr["SourceIPAddress"] = receivedRegister.SourceIPAddress == null ? DBNull.Value : receivedRegister.SourceIPAddress;
                                    dr["DestinationIPAddress"] = receivedRegister.DestinationIPAddress == null ? DBNull.Value : receivedRegister.DestinationIPAddress;
                                    dr["BrazilianTime"] = receivedRegister.BrazilianTime == null ? DBNull.Value : receivedRegister.BrazilianTime;
                                    dr["Url"] = receivedRegister.Url == null ? DBNull.Value : receivedRegister.Url;
                                    dr["StatusCode"] = receivedRegister.StatusCode == null ? DBNull.Value : receivedRegister.StatusCode;
                                    dr["RequestBytes"] = receivedRegister.RequestBytes == null ? DBNull.Value : receivedRegister.RequestBytes;

                                    tbl.Rows.Add(dr);

                                    if (commitCount >= 100000)
                                    {
                                        var connAux = new SqlConnection("Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=Database!2022;");
                                        var bcAux = new SqlBulkCopy(connAux);
                                        bcAux.DestinationTableName = "accessLog";
                                        try
                                        {
                                            bcAux.ColumnMappings.Add("SourceIPAddress", "SourceIPAddress");
                                            bcAux.ColumnMappings.Add("DestinationIPAddress", "DestinationIPAddress");
                                            bcAux.ColumnMappings.Add("BrazilianTime", "BrazilianTime");
                                            bcAux.ColumnMappings.Add("Url", "Url");
                                            bcAux.ColumnMappings.Add("StatusCode", "StatusCode");
                                            bcAux.ColumnMappings.Add("RequestBytes", "RequestBytes");

                                            connAux.Open();

                                            bcAux.WriteToServer(tbl);

                                            connAux.Close();

                                            Console.WriteLine($"Some lines were stored into database!");

                                            tbl.Clear();

                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                        commitCount = 0;
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("Invalid object received.");
                                }
                            }
                        }

                        Console.WriteLine("Storing into database...");

                        var conn = new SqlConnection("Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=Database!2022;");
                        var bc = new SqlBulkCopy(conn);
                        bc.DestinationTableName = "accessLog";
                        try
                        {
                            bc.ColumnMappings.Add("SourceIPAddress", "SourceIPAddress");
                            bc.ColumnMappings.Add("DestinationIPAddress", "DestinationIPAddress");
                            bc.ColumnMappings.Add("BrazilianTime", "BrazilianTime");
                            bc.ColumnMappings.Add("Url", "Url");
                            bc.ColumnMappings.Add("StatusCode", "StatusCode");
                            bc.ColumnMappings.Add("RequestBytes", "RequestBytes");

                            conn.Open();

                            bc.WriteToServer(tbl);

                            conn.Close();

                            Console.WriteLine($"The log was stored in the database succefully!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }).Start();
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