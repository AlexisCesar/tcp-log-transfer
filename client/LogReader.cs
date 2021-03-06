namespace client
{
    internal class LogReader
    {
        public void readAndPerformActionForEachLine(string path, Action<string> action)
        {
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    action(line);
                }
            }
        }
    }
}
