#include "stdafx.h"
#include "Frequency.h"

namespace edu
{
	namespace statistics
	{

		Frequency::Frequency() : _maxFrequency(0)
		{
		}


		Frequency::~Frequency()
		{
		}


		void Frequency::addValue(wstring &value)
		{
			map<wstring, size_t>::iterator iterator = _frequencyValues.find(value);
			if (_frequencyValues.end() != iterator)
			{
				size_t frequency = ++(iterator->second);
				if (_maxFrequency == frequency)
				{
					_modeValues.push_back(value);
				}
				else if (_maxFrequency < frequency)
				{
					_maxFrequency = frequency;
					_modeValues.clear();
					_modeValues.push_back(value);
				}
			}
			else
			{
				_frequencyValues[value] = 1;
				if (_maxFrequency == 1)
				{
					_modeValues.push_back(value);
				}
				else if (_maxFrequency < 1)
				{
					_maxFrequency = 1;
					_modeValues.push_back(value);
				}
			}
		}


		size_t Frequency::frequencyCount(wstring &value)
		{
			map<wstring, size_t>::iterator iterator = _frequencyValues.find(value);
			if (_frequencyValues.end() != iterator)
			{
				return iterator->second;
			}

			return 0;
		}


		vector<wstring>& Frequency::modeValues()
		{
			return _modeValues;
		}
	}
}