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

using System.Collections.ObjectModel;

namespace DataQualityApp.ViewModel
{
    /// <summary>
    /// Represents a simple view model.
    /// </summary>
    internal class DataQualityViewModel
    {
        private readonly ObservableCollection<FieldItem> _items;

        internal DataQualityViewModel()
        {
            _items = new ObservableCollection<FieldItem>();
            _items.Add(new FieldItem { Name = @"ID", Quality = 100 });
            _items.Add(new FieldItem { Name = @"Name", Quality = 75 });
            _items.Add(new FieldItem { Name = @"Aliasname", Quality = 50 });
            _items.Add(new FieldItem { Name = @"Alternatename", Quality = 25 });
            _items.Add(new FieldItem { Name = @"Editor", Quality = 0 });
        }

        public ObservableCollection<FieldItem> Items
        {
            get { return _items; }
        }
    }
}
