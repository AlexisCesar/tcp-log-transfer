namespace client
{
    internal class AccessLogProcessor : ILogProcessor
    {
        public void processLogLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
