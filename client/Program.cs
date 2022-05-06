using client;

var logReader = new LogReader();

var logProcessor = new AccessLogProcessor();

logReader.readAndPerformActionForEachLine(@"C:\dev\tcp-log-transfer\access.log", (line) => {
    logProcessor.processLogLine(line);
});