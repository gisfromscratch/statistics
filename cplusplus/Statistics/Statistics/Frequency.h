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

		private:
			void addValue(wstring &value);
		};

	}
}