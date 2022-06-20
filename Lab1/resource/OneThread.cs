namespace Lab1.resource
{
    internal class OneThread : Base
    {
        public override (string?, TimeSpan) start(string path)
        {
            List<Dictionary<string, int>> list = new List<Dictionary<string, int>>();
            startTime = DateTime.Now;
            foreach (string file in Directory.GetFiles(path))
            {
                list.Add(get_word_dict(file));
            }

            foreach (var v in list)
            {
                result = get_sum_dict(v, result);
            }

            return (convert(result), DateTime.Now - startTime);
        }
    }
}
