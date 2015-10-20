// Statistics.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace edu::statistics;
using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	for (int index = 1; index < argc; index++)
	{
		wstring argument = argv[index];
		wifstream inputStream(argument);
		if (inputStream)
		{
			wcout << L"Reading " << argument << endl;
			vector<wstring> fieldNames;
			map<wstring, Frequency> frequencies;

			// UTF converter
			locale utf8Locale(locale::empty(), new codecvt_utf8<wchar_t>());
			inputStream.imbue(utf8Locale);

			bool readHeader = true;

			wstring line;
			wregex splitter(L",");
			const size_t chunkSize = (size_t) 5e5;
			for (size_t lineNumber = 1; getline(inputStream, line); lineNumber++)
			{
				wsregex_token_iterator tokenIterator(line.begin(), line.end(), splitter, -1);
				wsregex_token_iterator tokenIteratorEnd;
				for (size_t tokenIndex = 0; tokenIteratorEnd != tokenIterator; tokenIndex++, tokenIterator++)
				{
					wstring nextToken = *tokenIterator;
					if (readHeader)
					{
						fieldNames.push_back(nextToken);
						frequencies[nextToken] = Frequency();
					}
					else
					{
						if (fieldNames.size() <= tokenIndex)
						{
							break;
						}

						wstring fieldName = fieldNames[tokenIndex];
						map<wstring, Frequency>::iterator iterator = frequencies.find(fieldName);
						if (frequencies.end() != iterator)
						{
							Frequency &frequency = iterator->second;
							frequency.addValue(nextToken);
						}
					}
				}

				readHeader = fieldNames.empty();
				if (0 == (lineNumber % chunkSize))
				{
					wcout << lineNumber << L" lines read." << endl;
				}
			}

			inputStream.close();

			wcout << endl;
			map<wstring, Frequency>::iterator frequenciesEnd = frequencies.end();
			for (map<wstring, Frequency>::iterator fieldEntryIterator = frequencies.begin(); frequenciesEnd != fieldEntryIterator; fieldEntryIterator++)
			{
				wstring fieldName = fieldEntryIterator->first;
				Frequency &frequency = fieldEntryIterator->second;
				wcout << fieldName << L"\t" << endl;
				vector<wstring> &modeValues = frequency.modeValues();
				vector<wstring>::iterator modeValuesEnd = modeValues.end();
				for (vector<wstring>::iterator modeValueIterator = modeValues.begin(); modeValuesEnd != modeValueIterator; modeValueIterator++)
				{
					wstring modeValue = *modeValueIterator;
					wcout << L"\t" << modeValue << L":\t" << frequency.frequencyCount(modeValue) << endl;
				}
			}
			wcout << endl;
		}
	}
	return 0;
}

