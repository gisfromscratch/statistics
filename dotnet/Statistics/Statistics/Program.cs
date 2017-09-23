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

        static void Main(string[] arguments)
        {
            foreach (var argument in arguments)
            {
                using (var reader = new StreamReader(File.OpenRead(argument)))
                {
                    Console.WriteLine(@"Reading {0}", argument);

                    var tool = new FrequencyTool(Settings.Default.HasHeader, GetDelimiters());
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
