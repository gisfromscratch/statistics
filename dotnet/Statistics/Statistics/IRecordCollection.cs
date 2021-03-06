﻿/*
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

namespace Statistics
{
    /// <summary>
    /// Represents a collection of records which can be accessed by an index.
    /// </summary>
    interface IRecordCollection : IDisposable
    {
        /// <summary>
        /// Adds a record to this collection and returns the record index.
        /// </summary>
        /// <param name="record">the record to add</param>
        /// <returns><see cref="int"/></returns>
        int Add(Record record);

        /// <summary>
        /// Gets the specified record.
        /// </summary>
        /// <param name="index">the index of the record</param>
        /// <returns><see cref="Record"/></returns>
        Record Get(int index);

        /// <summary>
        /// Returns the number of record of this collection.
        /// </summary>
        /// <returns><see cref="int"/></returns>
        int Count();
    }
}
