namespace Lab1.resource
{
    internal class FilePath
    {
        public static string getFilePath()
        {
            string path = Environment.CurrentDirectory;
            path = path.Replace(@"Lab1\bin\Debug\net6.0", @"Source\");
            return path;
        }
    }
}
