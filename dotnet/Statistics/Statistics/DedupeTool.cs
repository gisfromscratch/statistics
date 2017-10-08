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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IRecordCollection _records;
        private readonly int _blockIndex;
        private readonly int[] _compareIndices;
        private readonly IDictionary<string, IList<int>> _blocks;

        internal DedupeTool(bool hasHeader, char[] delims, int[] compareIndices, int blockIndex = -1)
        {
            _hasHeader = hasHeader;
            _delims = delims;
            _readHeader = true;
            _records = new SimpleRecordCollection();
            _blockIndex = blockIndex;
            _compareIndices = compareIndices;
            _blocks = new Dictionary<string, IList<int>>();
        }

        internal void Apply(string line)
        {
            var tokens = line.Split(_delims);
            var tokenCount = tokens.Length;
            var attributes = new List<Attribute>();
            string soundex = null;
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

                    // SoundEx-Blocking
                    if (tokenIndex == _blockIndex)
                    {
                        soundex = StringUtils.SoundEx(nextToken, 4);
                    }
                }
            }

            // Create a new record
            if (0 < attributes.Count)
            {
                var record = new Record(attributes);
                var recordIndex = _records.Add(record);

                // SoundEx-Bocking
                if (null != soundex)
                {
                    if (_blocks.ContainsKey(soundex))
                    {
                        _blocks[soundex].Add(recordIndex);
                    }
                    else
                    {
                        _blocks.Add(soundex, new List<int> { recordIndex });
                    }
                }
            }

            if (_readHeader)
            {
                _readHeader = false;
            }
        }

        internal void Summary()
        {
            const float threshold = 0.9f;
            var matches = new ConcurrentDictionary<Record, IList<Record>>();
            if (-1 == _blockIndex)
            {
                // Any combination
                var recordCount = _records.Count();
                Parallel.For(0, recordCount, recordIndex =>
                {
                    var record = _records.Get(recordIndex);
                    Interlocked.Increment(ref recordIndex);
                    Parallel.For(recordIndex, recordCount, otherRecordIndex =>
                    {
                        // Calculate similarities
                        var otherRecord = _records.Get(otherRecordIndex);
                        var attributes = record.Attributes;
                        var otherAttributes = otherRecord.Attributes;
                        var attributeCount = attributes.Count;
                        var otherAttributeCount = otherAttributes.Count;
                        if (attributeCount == otherAttributeCount)
                        {
                            var matchCount = 0;
                            if (_compareIndices.Length < 1)
                            {
                                // Compare all attributes
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
                                        matches[record] = new List<Record>() { otherRecord };
                                    }
                                }
                            }
                            else
                            {
                                // Compare only defined attributes
                                var maxMatchCount = _compareIndices.Length;
                                foreach (var index in _compareIndices)
                                {
                                    if (index < attributeCount)
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
                                }
                                if (matchCount == maxMatchCount)
                                {
                                    if (matches.ContainsKey(record))
                                    {
                                        matches[record].Add(otherRecord);
                                    }
                                    else
                                    {
                                        matches[record] = new List<Record>() { otherRecord };
                                    }
                                }
                            }
                        }
                    });
                });
            }
            else
            {
                // Only blocking records
                Parallel.ForEach(_blocks, block =>
                {
                    // Any combination for this block
                    var recordIndices = block.Value;
                    var recordCount = recordIndices.Count;
                    Parallel.For(0, recordCount, recordIndex =>
                    {
                        var record = _records.Get(recordIndex);
                        Interlocked.Increment(ref recordIndex);
                        Parallel.For(recordIndex, recordCount, otherRecordIndex =>
                        {
                            // Calculate similarities
                            var otherRecordIndexPosition = recordIndices[otherRecordIndex];
                            var otherRecord = _records.Get(otherRecordIndexPosition);
                            var attributes = record.Attributes;
                            var otherAttributes = otherRecord.Attributes;
                            var attributeCount = attributes.Count;
                            var otherAttributeCount = otherAttributes.Count;
                            if (attributeCount == otherAttributeCount)
                            {
                                var matchCount = 0;
                                if (_compareIndices.Length < 1)
                                {
                                    // Compare all attributes
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
                                            matches[record] = new List<Record>() { otherRecord };
                                        }
                                    }
                                }
                                else
                                {
                                    // Compare only defined attributes
                                    var maxMatchCount = _compareIndices.Length;
                                    foreach (var index in _compareIndices)
                                    {
                                        if (index < attributeCount)
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
                                    }
                                    if (matchCount == maxMatchCount)
                                    {
                                        if (matches.ContainsKey(record))
                                        {
                                            matches[record].Add(otherRecord);
                                        }
                                        else
                                        {
                                            matches[record] = new List<Record>() { otherRecord };
                                        }
                                    }
                                }
                            }
                        });
                    });
                });
            }

            // Release resources
            _records.Dispose();

#if DEBUG
#else
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
                Console.WriteLine();
            }
            Console.WriteLine();
#endif
        }
    }
}
