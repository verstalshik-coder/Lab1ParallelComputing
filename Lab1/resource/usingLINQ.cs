namespace Lab1.resource
{
    internal class usingLINQ : Base
    {
        public override (string?, TimeSpan) start(string path)
        {
            startTime = DateTime.Now;
            var list_of_dictionaries = Directory.GetFiles(path).Select(file => get_word_dict(file));
            result = list_of_dictionaries.Aggregate((dict1, dict2) => get_sum_dict(dict1, dict2));
            return (convert(result), DateTime.Now - startTime);
        }
    }
}
