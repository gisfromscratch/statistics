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
    /// Represents a simple deduplication model using SQLite.
    /// </summary>
    internal class DedupeModel
    {
        private readonly SQLiteConnection _connection;
        private string _insertPrefix;
        private int _attributeCount;

        internal DedupeModel()
        {
            _connection = new SQLiteConnection(@"Data Source=DedupeModel.db");
            _connection.Open();
        }

        internal void Initialize(IList<Attribute> attributes)
        {
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
        }

        internal void AddRecord(Record record)
        {
            // TODO: Use SQLiteParameter and bulk inserts
            var commandBuilder = new StringBuilder();
            commandBuilder.Append(_insertPrefix);
            commandBuilder.Append(@" VALUES (NULL");
            var attributes = record.Attributes;
            var attributeCount = attributes.Count;
            for (var attributeIndex = 0; attributeIndex < _attributeCount; attributeIndex++)
            {
                if (attributeIndex < attributeCount)
                {
                    var value = attributes[attributeIndex].Value;
                    commandBuilder.AppendFormat(@", '{0}'", value.Replace('\'', '#'));
                }
                else
                {
                    commandBuilder.Append(@", NULL");
                }
            }
            commandBuilder.Append(@");");
            using (var insertRecordCommand = new SQLiteCommand(_connection))
            {
                insertRecordCommand.CommandText = commandBuilder.ToString();
                insertRecordCommand.ExecuteNonQuery();
            }
        }
    }
}
