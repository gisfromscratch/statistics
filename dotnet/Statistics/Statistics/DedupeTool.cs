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
            for (var tokenIndex = 0; tokenIndex < tokenCount; tokenIndex++)
            {
                var nextToken = tokens[tokenIndex];
                if (_readHeader && _hasHeader)
                {
                    // Omit the header
                }
                else
                {
                    
                }
            }
            if (_readHeader)
            {
                _readHeader = false;
            }
        }
    }
}
