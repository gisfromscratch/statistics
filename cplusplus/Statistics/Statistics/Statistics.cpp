// Statistics.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

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

			// UTF converter
			locale utf8Locale(locale(), new codecvt_utf8<wchar_t>());
			inputStream.imbue(utf8Locale);

			bool readHeader = true;

			wstring line;
			wregex splitter(L",");
			while (getline(inputStream, line))
			{
				wsregex_token_iterator tokenIterator(line.begin(), line.end(), splitter, -1);
				wsregex_token_iterator tokenIteratorEnd;
				for (size_t tokenIndex = 0; tokenIteratorEnd != tokenIterator; tokenIterator++)
				{
					wstring token = *tokenIterator;
					if (readHeader)
					{
						fieldNames.push_back(token);
					}
					else
					{
						if (fieldNames.size() <= tokenIndex)
						{
							break;
						}
					}
				}

				readHeader = fieldNames.empty();
			}

			inputStream.close();
		}
	}
	return 0;
}

