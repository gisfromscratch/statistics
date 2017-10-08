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
    /// Represents a simple record collection using the .NET standard types.
    /// </summary>
    internal class SimpleRecordCollection : IRecordCollection
    {
        private readonly IList<Record> _records;

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        /// <param name="initialCapacity">the initial capacity</param>
        internal SimpleRecordCollection(int initialCapacity = 10000)
        {
            _records = new List<Record>(initialCapacity);
        }

        public int Add(Record record)
        {
            _records.Add(record);
            var recordIndex = Count() - 1;
            return recordIndex;
        }

        public Record Get(int index)
        {
            return _records[index];
        }

        public int Count()
        {
            return _records.Count;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SimpleRecordCollection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
