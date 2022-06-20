using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.resource
{
    internal class MultiThread : Base
    {
        public override (string?, TimeSpan) start(string path)
        {
            startTime = DateTime.Now;
            var list_of_dictionaries = Directory.GetFiles(path).AsParallel().WithDegreeOfParallelism(10).Select(file => get_word_dict(file));
            var result = list_of_dictionaries.Aggregate((dict1, dict2) => get_sum_dict(dict1, dict2));
            return (convert(result), DateTime.Now - startTime);
        }


    }
}
