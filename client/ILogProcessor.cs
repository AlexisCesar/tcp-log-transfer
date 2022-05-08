namespace client
{
    internal interface ILogProcessor<T>
    {
        void processLogLine(string line);
        T processLogLineAndReturnIt(string line);
    }
}
