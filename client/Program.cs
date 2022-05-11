using client;
using System.Text.Json;

var logReader = new LogReader();

var logProcessor = new AccessLogProcessor();

var processedLog = new List<AccessLogRegister>();

var connector = new TCPConnector();

Console.WriteLine("Processing log...");

logReader.readAndPerformActionForEachLine(@"C:\dev\tcp-log-transfer\access.log", (line) => {

    if (string.IsNullOrEmpty(line)) return;

    var register = logProcessor.processLogLineAndReturnIt(line);
    processedLog.Add(register);

});

Console.WriteLine("Log processed. Sending to server via TCP connection...");

processedLog.ForEach(x =>
{
    var serializedRegister = JsonSerializer.Serialize(x);
    connector.ConnectAndSendMessage("127.0.0.1", serializedRegister);
});