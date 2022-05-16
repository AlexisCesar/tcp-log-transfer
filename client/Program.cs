using System.Net.Sockets;
using System.Text.Json;
using client;

static void Connect(String server, IEnumerable<string> messages)
{
  try
  {
    // Create a TcpClient.
    Int32 port = 13000;
    TcpClient client = new TcpClient(server, port);

    // Get a client stream for reading and writing.
    NetworkStream stream = client.GetStream();
    StreamWriter sw = new StreamWriter(stream);

    foreach (string individualMessage in messages) {
        // Send the message to the connected TcpServer.
        sw.WriteLine(individualMessage);
    }
    

    // Close everything.
    sw.Close();
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
        if(String.IsNullOrEmpty(line)) continue;
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