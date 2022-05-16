// SERVER

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using server;

class MyTcpListener
{
  public static void Main()
  {
    TcpListener? server=null;
    try
    {
      // Set the TcpListener on port 13000.
      Int32 port = 13000;
      IPAddress localAddr = IPAddress.Parse("127.0.0.1");

      // TcpListener server = new TcpListener(port);
      server = new TcpListener(localAddr, port);

      // Start listening for client requests.
      server.Start();

      // Buffer for reading data
      Byte[] bytes = new Byte[256];
      String? data = null;

      // Enter the listening loop.
      while(true)
      {
        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected!");

        data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();
        StreamReader sr = new StreamReader(stream);

        //DATABASE TABLE
        DataTable dbTable = new DataTable();
        dbTable.Columns.Add(new DataColumn("Id", typeof(Int32)));
        dbTable.Columns.Add(new DataColumn("SourceIPAddress", typeof(string)));
        dbTable.Columns.Add(new DataColumn("DestinationIPAddress", typeof(string)));
        dbTable.Columns.Add(new DataColumn("BrazilianTime", typeof(DateTime)));
        dbTable.Columns.Add(new DataColumn("Url", typeof(string)));
        dbTable.Columns.Add(new DataColumn("StatusCode", typeof(Int32)));
        dbTable.Columns.Add(new DataColumn("RequestBytes", typeof(Int32)));
        //DATABASE TABLE

        int i;

        int messageCount = 0;

        //var messages = new List<AccessLogRegister>();

        // Loop to receive all the data sent by the client.
        while(sr.Peek() >= 0)
        {
          // Translate data bytes to a ASCII string.
          //data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
          data = sr.ReadLine();
          //Console.WriteLine("Received: {0}", data);
          messageCount++;
        
          //Console.WriteLine("Received: " + data);
          var register = JsonSerializer.Deserialize<AccessLogRegister>(data);

          if(register != null) {
              //messages.Add(register);
              DataRow dbRow = dbTable.NewRow();
                dbRow["SourceIPAddress"] = register.SourceIPAddress == null ? DBNull.Value : register.SourceIPAddress;
                dbRow["DestinationIPAddress"] = register.DestinationIPAddress == null ? DBNull.Value : register.DestinationIPAddress;
                dbRow["BrazilianTime"] = register.BrazilianTime == null ? DBNull.Value : register.BrazilianTime;
                dbRow["Url"] = register.Url == null ? DBNull.Value : register.Url;
                dbRow["StatusCode"] = register.StatusCode == null ? DBNull.Value : register.StatusCode;
                dbRow["RequestBytes"] = register.RequestBytes == null ? DBNull.Value : register.RequestBytes;
            dbTable.Rows.Add(dbRow);
          }

          // Process the data sent by the client.
          //data = data.ToUpper();

          //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

          // Send back a response.
          //stream.Write(msg, 0, msg.Length);
          //Console.WriteLine("Sent: {0}", data);
        }

        // Shutdown and end connection
        client.Close();
        Console.WriteLine($"Received {messageCount} messages.");
        messageCount = 0;

        // STORE DATA INTO DATABASE
          using(SqlConnection connection = new SqlConnection("Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=Database!2022;")){

            using(SqlBulkCopy bulkCopy = new SqlBulkCopy(connection)) {

                bulkCopy.DestinationTableName = "accessLog";

                bulkCopy.ColumnMappings.Add("SourceIPAddress", "SourceIPAddress");
                bulkCopy.ColumnMappings.Add("DestinationIPAddress", "DestinationIPAddress");
                bulkCopy.ColumnMappings.Add("BrazilianTime", "BrazilianTime");
                bulkCopy.ColumnMappings.Add("Url", "Url");
                bulkCopy.ColumnMappings.Add("StatusCode", "StatusCode");
                bulkCopy.ColumnMappings.Add("RequestBytes", "RequestBytes");

                connection.Open();

                bulkCopy.WriteToServer(dbTable);

                connection.Close();

                Console.WriteLine($"Some lines were stored into database!");

            }

          }
          // STORE DATA INTO DATABASE

      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
       // Stop listening for new clients.
       server.Stop();
    }

    Console.WriteLine("\nHit enter to continue...");
    Console.Read();
  }
}

