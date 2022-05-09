using client;

var logReader = new LogReader();

var logProcessor = new AccessLogProcessor();

var processedLog = new List<AccessLogRegister>();

var connector = new TCPConnector();

logReader.readAndPerformActionForEachLine(@"C:\dev\tcp-log-transfer\access.log", (line) => {
    //var register = logProcessor.processLogLineAndReturnIt(line);
    //processedLog.Add(register);
    logProcessor.processLogLine(line);
});

//processedLog.ForEach(x =>
//{
//    connector.Connect("127.0.0.1", 
//        $"Source: {x.SourceIPAddress}, Destination: {x.DestinationIPAddress}, URL: {x.Url}, BRT: {x.BrazilianTime}, StatusCode: {x.StatusCode}, Bytes: {x.RequestBytes}");
//});