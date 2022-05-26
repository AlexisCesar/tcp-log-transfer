using System.Diagnostics;
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

        foreach (string individualMessage in messages)
        {
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

Stopwatch stopwatch = new();
stopwatch.Start();

var messages = new List<string>();
var limitCount = 0;

long readLinesCount = 0;

using (FileStream fs = File.Open(@"C:\dev\tcp-log-transfer\access.log", FileMode.Open, FileAccess.Read, FileShare.Read))
using (BufferedStream bs = new BufferedStream(fs))
using (StreamReader sr = new StreamReader(bs))
{
    string line;
    var logProcessor = new AccessLogProcessor();

    while ((line = sr.ReadLine()) != null)
    {
        if (String.IsNullOrEmpty(line)) continue;
        // For each line of the log file:
        var myObj = logProcessor.processLogLineAndReturnIt(line);


        if (myObj == null)
        {
            continue;
        }

        var jsonObj = JsonSerializer.Serialize(myObj);

        if (jsonObj == null)
        {
            continue;
        }

        messages.Add(jsonObj);

        limitCount++;
        if (limitCount >= 200000)
        {
            //SEND Messages
            Console.WriteLine("Sending...");
            Connect("127.0.0.1", messages);

            Console.WriteLine($"Sent {messages.Count} messages.");
            messages.Clear();
            GC.Collect();
            //SEND Messages
            limitCount = 0;
        }

        readLinesCount++;
    }

}

Console.WriteLine($"Processed {readLinesCount} lines.");

Console.WriteLine("Sending...");
Connect("127.0.0.1", messages);

stopwatch.Stop();
TimeSpan elapsedTime = stopwatch.Elapsed;

Console.WriteLine($"Elapsed time: {elapsedTime.Hours}:{elapsedTime.Minutes}:{elapsedTime.Seconds}.{elapsedTime.Milliseconds}.");