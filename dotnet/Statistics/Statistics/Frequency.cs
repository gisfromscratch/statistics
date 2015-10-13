using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics
{
    /// <summary>
    /// Represents the frequency statistics.
    /// </summary>
    internal class Frequency<TValue> where TValue : class
    {
        private readonly IDictionary<IComparable<TValue>, long> _frequencyValues;

        private readonly IList<IComparable<TValue>> _modeValues;

        private long _maxFrequency;

        internal Frequency()
        {
            _frequencyValues = new Dictionary<IComparable<TValue>, long>();
            _modeValues = new List<IComparable<TValue>>();
        }

        internal void AddValue(IComparable<TValue> value)
        {
            if (_frequencyValues.ContainsKey(value))
            {
                var frequency = ++_frequencyValues[value];
                if (_maxFrequency == frequency)
                {
                    _modeValues.Add(value);
                }
                else if (_maxFrequency < frequency)
                {
                    _maxFrequency = frequency;
                    _modeValues.Clear();
                    _modeValues.Add(value);
                }
            }
            else
            {
                _frequencyValues.Add(value, 1);
                if (_maxFrequency == 1)
                {
                    _modeValues.Add(value);
                }
                else if (_maxFrequency < 1)
                {
                    _maxFrequency = 1;
                    _modeValues.Add(value);
                }
            }
        }

        internal long GetFrequencyCount(IComparable<TValue> value)
        {
            if (_frequencyValues.ContainsKey(value))
            {
                return _frequencyValues[value];
            }

            return 0;
        }

        internal IList<IComparable<TValue>> ModeValues
        {
            get
            {
                return _modeValues;
            }
        }
    }
}
