// CLIENT

using System.Net.Sockets;
using System.Text.Json;
using client;

static void Connect(String server, IEnumerable<string> messages)
{
  try
  {
    // Create a TcpClient.
    // Note, for this client to work you need to have a TcpServer
    // connected to the same address as specified by the server, port
    // combination.
    Int32 port = 13000;
    TcpClient client = new TcpClient(server, port);

    // Get a client stream for reading and writing.
    //  Stream stream = client.GetStream();

    NetworkStream stream = client.GetStream();
    StreamWriter sr = new StreamWriter(stream);

    foreach (string individualMessage in messages) {
        // Translate the passed message into ASCII and store it as a Byte array.
        //Byte[] data = System.Text.Encoding.ASCII.GetBytes(individualMessage);

        // Send the message to the connected TcpServer.
        sr.WriteLine(individualMessage);
        //sr.Flush();
        //Console.WriteLine("Sent: {0}", individualMessage);

        // Receive the TcpServer.response.

        // Buffer to store the response bytes.
        //data = new Byte[256];

        // String to store the response ASCII representation.
        //String responseData = String.Empty;

        // Read the first batch of the TcpServer response bytes.
        //Int32 bytes = stream.Read(data, 0, data.Length);
        //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //Console.WriteLine("Received: {0}", responseData);
    }
    sr.Close();

    // Close everything.
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

// ----------------------------------

var messages = new List<string>();
var limitCount = 0;

// Elapsed time vars
TimeSpan elapsedTime;
DateTime startTime, endTime;

long readLinesCount = 0;

using (FileStream fs = File.Open(@"C:\dev\tcp-log-transfer\access.log", FileMode.Open, FileAccess.Read, FileShare.Read))
using (BufferedStream bs = new BufferedStream(fs))
using (StreamReader sr = new StreamReader(bs))
{
    string line;
    var logProcessor = new AccessLogProcessor();

    while ((line = sr.ReadLine()) != null)
    {
        // For each line of the log file:
        var myObj = logProcessor.processLogLineAndReturnIt(line);


        if (myObj == null) {
            continue;
        }

        var jsonObj = JsonSerializer.Serialize(myObj);

        if (jsonObj == null) {
            continue;
        }

        messages.Add(jsonObj);

        limitCount++;
        if(limitCount >= 100000) {
            //SEND Messages
            startTime = DateTime.Now;

            Console.WriteLine("Sending...");
            Connect("127.0.0.1", messages);

            endTime = DateTime.Now;
            elapsedTime = endTime.Subtract(startTime);

            Console.WriteLine($"Sent {messages.Count} messages. In {elapsedTime} milliseconds.");
            messages.Clear();
            GC.Collect();
            //SEND Messages
            limitCount = 0;
        }

        readLinesCount++;
    }

}

Console.WriteLine($"Processed {readLinesCount} lines.");

startTime = DateTime.Now;

Console.WriteLine("Sending...");
Connect("127.0.0.1", messages);

endTime = DateTime.Now;

elapsedTime = endTime.Subtract(startTime);

Console.WriteLine($"Sent {messages.Count} messages. In {elapsedTime} milliseconds.");