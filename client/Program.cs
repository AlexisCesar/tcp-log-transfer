using client;

var logReader = new LogReader();

var logProcessor = new AccessLogProcessor();

var processedLog = new List<AccessLogRegister>();

logReader.readAndPerformActionForEachLine(@"C:\dev\tcp-log-transfer\access.log", (line) => {
    var register = logProcessor.processLogLineAndReturnIt(line);
    processedLog.Add(register);
});

processedLog.ForEach(x =>
{
    Console.WriteLine($"Source: {x.SourceIPAddress}, Destination: {x.DestinationIPAddress}, URL: {x.Url}, BRT: {x.BrazilianTime}, StatusCode: {x.StatusCode}, Bytes: {x.RequestBytes}");
});