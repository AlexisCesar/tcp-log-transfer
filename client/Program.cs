using client;
using System.Text.Json;

var logReader = new LogReader();
var logProcessor = new AccessLogProcessor();

var processedLog = new List<AccessLogRegister>();
var serializedLog = new List<string>();

var tcpConnector = new TCPConnector();

Console.WriteLine("Processing log...");

var processedLinesCounter = 0;

logReader.readAndPerformActionForEachLine(@"C:\dev\tcp-log-transfer\access.log", (line) => {

    if (string.IsNullOrEmpty(line)) return;

    var register = logProcessor.processLogLineAndReturnIt(line);
    processedLog.Add(register);

    processedLinesCounter++;

    if(processedLinesCounter >= 100000)
    {
        Console.WriteLine("Sending 100k registers to the server...");

        processedLog.ForEach(x =>
        {
            var serializedRegister = JsonSerializer.Serialize(x);
            serializedLog.Add(serializedRegister);
        });

        tcpConnector.ConnectAndSendListOfMessages("127.0.0.1", serializedLog);
        Console.WriteLine("100k lines sent.\n");

        processedLinesCounter = 0;
        serializedLog.Clear();
        processedLog.Clear();
        GC.Collect();
    }

});

Console.WriteLine("Sending last registers to the server via TCP connection...");

processedLog.ForEach(x =>
{
    var serializedRegister = JsonSerializer.Serialize(x);
    serializedLog.Add(serializedRegister);
});

tcpConnector.ConnectAndSendListOfMessages("127.0.0.1", serializedLog);

Console.WriteLine("\nLog has been sent to the server!");
Console.WriteLine("Exiting application...");
GC.Collect();