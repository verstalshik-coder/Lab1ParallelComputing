using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.resource
{
    internal abstract class Base
    {
        protected DateTime startTime;
        protected Dictionary<string, int> result = new Dictionary<string, int>();

        public abstract (string?, TimeSpan) start(string path);

        protected Dictionary<string, int> get_word_dict(string path)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (string line in File.ReadAllLines(path))
            {
                if (dictionary.ContainsKey(line))
                {
                    dictionary[line] = dictionary[line] + 1;
                }
                else
                {
                    dictionary[line] = 1;
                }
            }
            return dictionary;
        }

        protected Dictionary<string, int> get_sum_dict(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var key in dict1.Keys)
            {
                if (dict2.ContainsKey(key))
                    result[key] = dict1[key] + dict2[key];
                else
                    result[key] = dict1[key];
            }
            return result;
        }

        public string convert(Dictionary<string, int> dictionary)
        {
            string line = "";
            foreach (KeyValuePair<string, int> pair in dictionary)
            {
                line += "word: " + pair.Key + " count: " + pair.Value + "\n";
            }
            return line;
        }
    }
}
