using client;

var logReader = new LogReader();

logReader.readAndPerformActionForEachLine(@"C:\dev\tcp-log-transfer\access.log", (line) => { 
    Console.WriteLine(line);
});