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
                    IList<string> fieldNames = new List<string>();
                    IDictionary<string, Frequency<string>> frequencies = new Dictionary<string, Frequency<string>>();
                    bool readHeader = true;
                    string line;
                    const long chunkSize = (long) 5e5;
                    for (var lineNumber = 1; null != (line = reader.ReadLine()); lineNumber++)
                    {
                        var tokens = line.Split(GetDelimiters());
                        var tokenCount = tokens.Length;
                        for (var tokenIndex = 0; tokenIndex < tokenCount; tokenIndex++)
                        {
                            var nextToken = tokens[tokenIndex];
                            if (readHeader)
                            {
                                if (Settings.Default.HasHeader)
                                {
                                    var fieldName = nextToken;
                                    var fieldPrefix = 2;
                                    while (frequencies.ContainsKey(fieldName))
                                    {
                                        fieldName = string.Format(@"{0}_{1}", nextToken, fieldPrefix++);
                                    }
                                    fieldNames.Add(fieldName);
                                    frequencies.Add(fieldName, new Frequency<string>());
                                }
                                else
                                {
                                    var fieldName = string.Format(@"F_{0}", tokenIndex + 1);
                                    fieldNames.Add(fieldName);
                                    frequencies.Add(fieldName, new Frequency<string>());
                                }
                            }
                            else
                            {
                                if (fieldNames.Count <= tokenIndex)
                                {
                                    break;
                                }

                                var fieldName = fieldNames[tokenIndex];
                                if (frequencies.ContainsKey(fieldName))
                                {
                                    var frequency = frequencies[fieldName];
                                    frequency.AddValue(nextToken);
                                }
                            }
                        }
                        if (readHeader)
                        {
                            readHeader = !fieldNames.Any();
                        }
                        if (0 == (lineNumber % chunkSize))
                        {
                            Console.WriteLine(@"{0} lines read.", lineNumber);
                        }
                    }

                    Console.WriteLine();
                    foreach (var fieldEntry in frequencies) 
                    {
                        var fieldName = fieldEntry.Key;
                        var frequency = fieldEntry.Value;
                        Console.WriteLine("{0}\t", fieldName);
                        var modeValues = frequency.SortedModeValues;
                        var maxCount = 10;
                        var index = 0;
                        foreach (var modeValue in modeValues)
                        {
                            Console.WriteLine("\t{0}:\t{1}", modeValue, frequency.GetFrequencyCount(modeValue));
                            if (++index == maxCount)
                            {
                                break;
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
