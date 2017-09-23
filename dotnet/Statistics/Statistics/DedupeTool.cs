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

namespace Statistics
{
    /// <summary>
    /// Represents a simple deduplication tool.
    /// </summary>
    internal class DedupeTool
    {
        private readonly bool _hasHeader;
        private readonly char[] _delims;
        private bool _readHeader;
        private readonly IList<Record> _records;

        internal DedupeTool(bool hasHeader, char[] delims)
        {
            _hasHeader = hasHeader;
            _delims = delims;
            _readHeader = true;
            _records = new List<Record>();
        }

        internal void Apply(string line)
        {
            var tokens = line.Split(_delims);
            var tokenCount = tokens.Length;
            var attributes = new List<Attribute>();
            for (var tokenIndex = 0; tokenIndex < tokenCount; tokenIndex++)
            {
                var nextToken = tokens[tokenIndex];
                if (_readHeader && _hasHeader)
                {
                    // Omit the header
                }
                else
                {
                    var attribute = new Attribute();
                    attribute.AttributeType = AttributeType.String;
                    attribute.Value = nextToken;
                    attributes.Add(attribute);
                }
            }

            // Create a new record
            if (0 < attributes.Count)
            {
                var record = new Record(attributes);
                _records.Add(record);
            }

            if (_readHeader)
            {
                _readHeader = false;
            }
        }

        internal void Summary()
        {
            const float threshold = 0.9f;
            var matches = new Dictionary<Record, IList<Record>>();
            var recordCount = _records.Count;
            for (var recordIndex = 0; recordIndex < recordCount; recordIndex++)
            {
                var record = _records[recordIndex];
                for (var otherRecordIndex = recordIndex + 1; otherRecordIndex < recordCount; otherRecordIndex++)
                {
                    // Calculate similarities
                    var otherRecord = _records[otherRecordIndex];
                    var attributes = record.Attributes;
                    var otherAttributes = otherRecord.Attributes;
                    var attributeCount = attributes.Count;
                    var otherAttributeCount = otherAttributes.Count;
                    if (attributeCount == otherAttributeCount)
                    {
                        var matchCount = 0;
                        for (var index = 0; index < attributeCount; index++)
                        {
                            var value = attributes[index].Value;
                            var otherValue = otherAttributes[index].Value;
                            var similiarity = StringUtils.Similarity(value, otherValue);
                            if (threshold <= similiarity)
                            {
                                matchCount++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (matchCount == attributeCount)
                        {
                            if (matches.ContainsKey(record))
                            {
                                matches[record].Add(otherRecord);
                            }
                            else
                            {
                                matches.Add(record, new List<Record>() { otherRecord });
                            }
                        }
                    }
                }
            }

            // Print matches
            Console.WriteLine();
            foreach (var match in matches)
            {
                var record = match.Key;
                foreach (var attribute in record.Attributes)
                {
                    Console.Write(@"{0}|", attribute.Value);
                }
                Console.WriteLine();

                var otherRecords = match.Value;
                foreach (var otherRecord in otherRecords)
                {
                    foreach (var attribute in otherRecord.Attributes)
                    {
                        Console.Write(@"{0}|", attribute.Value);
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}
