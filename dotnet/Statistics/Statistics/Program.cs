using Statistics.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Statistics
{
    class Program
    {
        private static char[] GetDelimiters()
        {
            var delimiter = Settings.Default.Delimiter;
            switch (delimiter.ToUpperInvariant())
            {
                case @"TAB":
                    return new[] { '\t' };
                default:
                    return delimiter.ToCharArray();
            }
        }

        private static int[] GetCompareIndices()
        {
            var indices = new List<int>();
            foreach(var index in Settings.Default.CompareIndices)
            {
                int intValue;
                if (int.TryParse(index, out intValue))
                {
                    indices.Add(intValue);
                }
            }
            return indices.ToArray();
        }

        static void Main(string[] arguments)
        {
            foreach (var argument in arguments)
            {
                using (var reader = new StreamReader(File.OpenRead(argument)))
                {
                    Console.WriteLine(@"Reading {0}", argument);

                    var blockIndex = Settings.Default.BlockIndex;
                    var compareIndices = GetCompareIndices();
                    var tool = new DedupeTool(Settings.Default.HasHeader, GetDelimiters(), compareIndices, blockIndex);
                    string line;
                    const long chunkSize = (long)5e5;
                    for (var lineNumber = 1; null != (line = reader.ReadLine()); lineNumber++)
                    {
                        tool.Apply(line);
                        if (0 == (lineNumber % chunkSize))
                        {
                            Console.WriteLine(@"{0} lines read.", lineNumber);
                        }
                    }
                    tool.Summary();
                }
            }
        }
    }
}
