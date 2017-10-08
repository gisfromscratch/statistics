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
using System.Data.SQLite;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Represents a simple record collection using a SQLite table.
    /// </summary>
    internal class SqliteRecordCollection : IRecordCollection
    {
        private SQLiteConnection _connection;
        private SQLiteTransaction _transaction;
        private SQLiteCommand _insertCommand;
        private readonly int _autoCommitSize;
        private readonly object _commitLock;
        private int _newRecordCount;
        private int _recordCount;
        private IList<Attribute> _attributes;
        private IList<SQLiteParameter> _insertParameters;
        private string _insertPrefix;
        private int _attributeCount;

        internal SqliteRecordCollection(int initialCapacity = 10000)
        {
            _connection = new SQLiteConnection(@"Data Source=DedupeModel.db");
            _connection.Open();
            _insertParameters = new List<SQLiteParameter>();
            _autoCommitSize = initialCapacity;
            _commitLock = new object();
        }

        private void Initialize(IList<Attribute> attributes)
        {
            // Drop table
            using (var dropCommand = new SQLiteCommand(_connection))
            {
                dropCommand.CommandText = @"DROP TABLE IF EXISTS Records;";
                dropCommand.ExecuteNonQuery();
            }

            var commandBuilder = new StringBuilder();
            commandBuilder.Append(@"CREATE TABLE IF NOT EXISTS Records (ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT");
            // TODO: Truncate table
            var insertBuilder = new StringBuilder();
            insertBuilder.Append(@"INSERT INTO Records (ID");
            _attributeCount = attributes.Count;
            for(var attributeIndex = 0; attributeIndex < _attributeCount; attributeIndex++)
            {
                var attribute = attributes[attributeIndex];
                var fieldName = string.Format(@"ATTRIBUTE_{0}", attributeIndex + 1);
                switch (attribute.AttributeType)
                {
                    default:
                        commandBuilder.AppendFormat(@", {0} VARCHAR(1000)", fieldName);
                        insertBuilder.AppendFormat(@", {0}", fieldName);
                        break;
                }
            }
            commandBuilder.Append(@");");
            insertBuilder.Append(@")");
            _insertPrefix = insertBuilder.ToString();
            using (var createRecordTableCommand = new SQLiteCommand(_connection))
            {
                createRecordTableCommand.CommandText = commandBuilder.ToString();
                createRecordTableCommand.ExecuteNonQuery();
            }

            // Create a transaction
            _transaction = _connection.BeginTransaction();
            _attributes = attributes;
        }

        internal void Flush()
        {
            if (null != _transaction)
            {
                _transaction.Commit();
                _transaction = _connection.BeginTransaction();
                _newRecordCount = 0;
            }
        }

        internal void Release()
        {
            // Lock on each commit
            lock (_commitLock)
            {
                if (0 < _newRecordCount)
                {
                    Flush();
                }
            }

            if (null != _insertCommand)
            {
                _insertCommand.Dispose();
                _insertCommand = null;
            }

            if (null != _transaction)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (null != _connection)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        public int Add(Record record)
        {
            var attributes = record.Attributes;

            // Use SQLiteParameter and bulk inserts
            if (null == _insertCommand)
            {
                Initialize(attributes);
                _insertCommand = new SQLiteCommand(_connection);

                var commandBuilder = new StringBuilder();
                commandBuilder.Append(_insertPrefix);
                commandBuilder.Append(@" VALUES (NULL");
                for (var attributeIndex = 0; attributeIndex < _attributeCount; attributeIndex++)
                {
                    var parameterName = string.Format(@":ATTRIBUTE_{0}", attributeIndex + 1);
                    commandBuilder.AppendFormat(@", {0}", parameterName);
                    var insertParameter = new SQLiteParameter();
                    insertParameter.ParameterName = parameterName;
                    insertParameter.DbType = System.Data.DbType.String;
                    _insertCommand.Parameters.Add(insertParameter);
                    _insertParameters.Add(insertParameter);
                }
                commandBuilder.Append(@");");
                _insertCommand.CommandText = commandBuilder.ToString();
            }

            var attributesCount = attributes.Count;
            for (var attributeIndex = 0; attributeIndex < _attributeCount; attributeIndex++)
            {
                var parameter = _insertParameters[attributeIndex];
                if (attributeIndex < attributesCount)
                {
                    parameter.Value = attributes[attributeIndex].Value;
                }
                else
                {
                    //parameter.Value = DBNull.Value;
                    parameter.Value = null;
                }
            }

            // Execute the query
            _insertCommand.ExecuteNonQuery();

            // Lock on each commit
            lock (_commitLock)
            {
                _recordCount++;
                _newRecordCount++;
                if (_autoCommitSize <= _newRecordCount)
                {
                    Flush();
                }
            }

            return _recordCount - 1;
        }

        public Record Get(int index)
        {
            // Lock on each commit
            lock (_commitLock)
            {
                // Commit all new records
                if (0 < _newRecordCount)
                {
                    Flush();
                }
            }

            var attributes = new List<Attribute>(_attributeCount);
            foreach (var attribute in _attributes)
            {
                var clonedAttribute = new Attribute { AttributeType = attribute.AttributeType };
                attributes.Add(clonedAttribute);
            }
            var record = new Record(attributes);

            // Query the table
            using (var selectCommand = new SQLiteCommand(_connection))
            {
                var id = index + 1;
                selectCommand.CommandText = string.Format(@"SELECT * FROM Records WHERE ID={0}", id);
                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (var attributeIndex = 0; attributeIndex < _attributeCount; attributeIndex++)
                        {
                            var attribute = attributes[attributeIndex];
                            var value = reader.GetFieldValue<string>(attributeIndex + 1);
                            attribute.Value = value;
                        }
                    }
                }
            }
            return record;
        }

        public int Count()
        {
            return _recordCount;

            //// Lock on each commit
            //lock (_commitLock)
            //{
            //    // Commit all new records
            //    if (0 < _newRecordCount)
            //    {
            //        Flush();
            //    }
            //}

            //using (var countCommand = new SQLiteCommand(_connection))
            //{
            //    countCommand.CommandText = @"SELECT COUNT(*) FROM Records";
            //    var count = (int) countCommand.ExecuteScalar();
            //    return count;
            //}
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    Release();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SqliteRecordCollection() {
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
