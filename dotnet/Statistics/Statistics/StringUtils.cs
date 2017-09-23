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
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Utility functions operating on strings.
    /// </summary>
    public static class StringUtils
    {
        private const float Unequal = 0.0f;
        private const float Missing = 0.5f;
        private const float Identical = 1.0f;

        /// <summary>
        /// Creates the SOUNDEX code for the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SoundEx(string text, ushort length)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var characters = text.ToUpperInvariant().ToCharArray();
            var codeLength = Math.Min(characters.Length, length);
            var buffer = new StringBuilder();
            buffer.Append(characters[0]);
            for (var index = 1; index <= codeLength; index++)
            {
                switch (characters[index])
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U':
                    case 'H':
                    case 'W':
                    case 'Y':
                        buffer.Append("0");
                        break;

                    case 'B':
                    case 'F':
                    case 'P':
                    case 'V':
                        buffer.Append("1");
                        break;

                    case 'C':
                    case 'G':
                    case 'J':
                    case 'K':
                    case 'Q':
                    case 'S':
                    case 'X':
                    case 'Z':
                        buffer.Append("2");
                        break;

                    case 'D':
                    case 'T':
                        buffer.Append("3");
                        break;

                    case 'L':
                        buffer.Append("4");
                        break;

                    case 'M':
                    case 'N':
                        buffer.Append("5");
                        break;

                    case 'R':
                        buffer.Append("6");
                        break;

                    default:
                        buffer.Append("0");
                        break;
                }
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Calculates the similarity between two strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="otherText"></param>
        /// <returns></returns>
        public static float Similarity(string text, string otherText)
        {
            if (null == text || null == otherText)
            {
                return Unequal;
            }

            int length = text.Length;
            int otherLength = otherText.Length;
            if (0 == length && 0 == otherLength)
            {
                return Missing;
            }
            else if (0 == length || 0 == otherLength)
            {
                return Unequal;
            }

            if (otherLength < length)
            {
                // Swaq strings to comsume less memory
                string temp = text;
                text = otherText;
                otherText = temp;
                length = otherLength;
                otherLength = otherText.Length;
            }

            // Initialize distance vector
            int[] distances = new int[length + 1];
            for (var index = 0; index <= length; index++)
            {
                distances[index] = index;
            }

            // Calculate distance
            for (var otherIndex = 1; otherIndex <= otherLength; otherIndex++)
            {
                int upperLeft = distances[0];
                char otherCharacter = otherText[otherIndex - 1];
                distances[0] = otherIndex;
                for (var index = 1; index <= length; index++)
                {
                    int upper = distances[index];
                    int cost = text[index - 1] == otherCharacter ? 0 : 1;
                    distances[index] = Math.Min(Math.Min(distances[index - 1] + 1, distances[index] + 1), upperLeft + cost);
                    upperLeft = upper;
                }
            }
            return Identical - (1.0f * distances[length] / otherLength);
        }
    }
}
