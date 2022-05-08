namespace client
{
    internal class AccessLogProcessor : ILogProcessor
    {
        public void processLogLine(string line)
        {
            Console.WriteLine(line);
        }

        public AccessLogRegister processLogLineAndReturnLogRegister(string line)
        {
            return new AccessLogRegister();
        }
    }
}
