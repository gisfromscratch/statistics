// Clustering.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;

#define __cplusplus 1


static void showCalculateDistances()
{
	const int nRows = 2;
	const int nColumns = 1;
	double data[2][1] = { { 0.0 }, { 1.0 } };
	double *dataPointers[2];
	int dataFlags[2][1] = { { 1 }, { 1 } };
	int *dataFlagPointers[2];
	for (int i = 0; i < nRows; i++)
	{
		dataPointers[i] = data[i];
		dataFlagPointers[i] = dataFlags[i];
	}

	double dataWeights[2] = { 1.0, 1.0 };
	int nRows1 = 1;
	int indices1[] = { 0 };
	int nRows2 = 1;
	int indices2[] = { 1 };

	char distanceFunc = 'e'; // Euclidean
	char distanceMethod = 'm'; // Median

	int transpose = 0;

	double distance = clusterdistance(nRows, nColumns, dataPointers, dataFlagPointers, dataWeights, nRows1, nRows2, indices1, indices2, distanceFunc, distanceMethod, transpose);
	wcout << L"Distance " << distance << endl << endl;
}


int _tmain(int argc, _TCHAR* argv[])
{
	showCalculateDistances();
	system("PAUSE");
	return 0;
}

