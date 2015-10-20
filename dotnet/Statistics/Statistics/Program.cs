using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics
{
    class Program
    {
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
                        var tokens = line.Split(',');
                        var tokenCount = tokens.Length;
                        for (var tokenIndex = 0; tokenIndex < tokenCount; tokenIndex++)
                        {
                            var nextToken = tokens[tokenIndex];
                            if (readHeader)
                            {
                                fieldNames.Add(nextToken);
                                frequencies.Add(nextToken, new Frequency<string>());
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
                        var modeValues = frequency.ModeValues;
                        foreach (var modeValue in modeValues)
                        {
                            Console.WriteLine("\t{0}:\t{1}", modeValue, frequency.GetFrequencyCount(modeValue));
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
