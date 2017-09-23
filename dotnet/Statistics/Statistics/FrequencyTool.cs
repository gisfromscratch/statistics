/*
 * Copyright 2017 Jan Tschada
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Statistics
{
    /// <summary>
    /// Represents a simple frequency tool.
    /// </summary>
    internal class FrequencyTool
    {
        private readonly bool _hasHeader;
        private readonly char[] _delims;
        private bool _readHeader;
        private readonly IList<string> _fieldNames;
        private readonly IDictionary<string, Frequency<string>> _frequencies;

        internal FrequencyTool(bool hasHeader, char[] delims)
        {
            _hasHeader = hasHeader;
            _delims = delims;
            _readHeader = true;
            _fieldNames = new List<string>();
            _frequencies = new Dictionary<string, Frequency<string>>();
        }

        internal void Apply(string line)
        {
            var tokens = line.Split(_delims);
            var tokenCount = tokens.Length;
            for (var tokenIndex = 0; tokenIndex < tokenCount; tokenIndex++)
            {
                var nextToken = tokens[tokenIndex];
                if (_readHeader)
                {
                    if (_hasHeader)
                    {
                        // First line is the header
                        var fieldName = nextToken;
                        var fieldPrefix = 2;
                        while (_frequencies.ContainsKey(fieldName))
                        {
                            fieldName = string.Format(@"{0}_{1}", nextToken, fieldPrefix++);
                        }
                        _fieldNames.Add(fieldName);
                        _frequencies.Add(fieldName, new Frequency<string>());
                    }
                    else
                    {
                        // First line is not the header
                        var fieldName = string.Format(@"F_{0}", tokenIndex + 1);
                        _fieldNames.Add(fieldName);
                        var frequency = new Frequency<string>();
                        frequency.AddValue(nextToken);
                        _frequencies.Add(fieldName, frequency);
                    }
                }
                else
                {
                    if (_fieldNames.Count <= tokenIndex)
                    {
                        break;
                    }

                    var fieldName = _fieldNames[tokenIndex];
                    if (_frequencies.ContainsKey(fieldName))
                    {
                        var frequency = _frequencies[fieldName];
                        frequency.AddValue(nextToken);
                    }
                }
            }
            if (_readHeader)
            {
                _readHeader = !_fieldNames.Any();
            }
        }

        internal void Summary()
        {
            Console.WriteLine();
            foreach (var fieldEntry in _frequencies)
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
