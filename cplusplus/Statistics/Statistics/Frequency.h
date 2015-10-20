#pragma once

#include <string>

using namespace std;

namespace edu
{
	namespace statistics
	{
		class Frequency
		{
		public:
			Frequency();
			~Frequency();

			void addValue(wstring &value);

			size_t frequencyCount(wstring &value);

			vector<wstring>& modeValues();

		private:
			map<wstring, size_t> _frequencyValues;
			vector<wstring> _modeValues;
			size_t _maxFrequency;
		};
	}
}