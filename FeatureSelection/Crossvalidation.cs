using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FeatureSelection
{
	class Crossvalidation
	{
		public static void SpiltData(string fileName, int fold)
		{

			bool UseExistingCvFiles = true;
			bool _noiseInput = false;
			bool _noiseOutput = false;
			double NoiseFrequencyIn = 0.0, NoiseFrequencyOut = 0.0, NoiseValue = 0.0;


			string[] _trainingFileNames = new string[fold];
			string[] _testFileNames = new string[fold];
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			string fns = Path.GetDirectoryName(fileName) + "\\" + fileNameWithoutExtension;
			if (!Directory.Exists(fns))
				Directory.CreateDirectory(fns);
			string fnx = fns + "\\" + fileNameWithoutExtension;

			for (int f = 0; f < fold; f++)
			{
				_trainingFileNames[f] = fnx + "_" + fold + "_" + f + "_trn" + ".txt";
				_testFileNames[f] = fnx + "_" + fold + "_" + f + "_tst" + ".txt";
			}


			if (UseExistingCvFiles)
			{
				int numExistingFiles = 0;
				for (int f = 0; f < fold; f++)
				{
					if (File.Exists(_trainingFileNames[f]) && File.Exists(_testFileNames[f]))
					{
						numExistingFiles += 2;
					}

					if (numExistingFiles == 2 * fold)
						return;
				}
			}


			Random R = new Random();
			double r;

			string[] originalFile = File.ReadAllLines(fileName);
			string _headerLine = originalFile[0];
			bool Classification = _headerLine.Trim().ToLower().Contains(" class");

			int _numVectors = originalFile.Length - 1;
			string[] nA = _headerLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			int _numAttributes = nA.Length;

			double[][] dataSet = new double[_numVectors][];
			for (int v = 0; v < _numVectors; v++)
				dataSet[v] = new double[_numAttributes];

			if (_noiseInput || _noiseOutput)
			{
				for (int v = 0; v < _numVectors; v++)
				{
					nA = originalFile[v + 1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					for (int a = 0; a < _numAttributes; a++)
						dataSet[v][a] = Convert.ToDouble(nA[a]);
				}
			}






			int[] steps = new int[fold + 1];
			for (int s = 0; s < fold; s++)
				steps[s] = s * _numVectors / fold;
			steps[fold] = _numVectors;

			for (int f = 0; f < fold; f++)
			{

				using (StreamWriter trainingStreamWriter = new StreamWriter(_trainingFileNames[f]))
				using (StreamWriter testStreamWriter = new StreamWriter(_testFileNames[f]))
				{
					trainingStreamWriter.WriteLine(_headerLine);
					testStreamWriter.WriteLine(_headerLine);

					//  double d = 0;
					if (_noiseInput || _noiseOutput)
					{
						for (int v = 0; v < _numVectors; v++)
						{
							if (v >= steps[f] && v < steps[f + 1]) //write these vectors to the test set
							{
								// if (v >= steps[f] && v < steps[f + 1]) //write these vectors to the test set                          
								testStreamWriter.WriteLine(originalFile[v + 1]);
							}
							else //write these vectors to the training set
							{
								if (_noiseInput)
								{
									if ((r = R.NextDouble()) < NoiseFrequencyIn)
									{
										for (int a = 0; a < _numAttributes - 1; a++)
											trainingStreamWriter.Write($"{(dataSet[v][a] + NoiseValue * 1.72 * (R.NextDouble() + R.NextDouble() + R.NextDouble() + R.NextDouble() - 2)):F5} ");
									}
									else
									{
										for (int a = 0; a < _numAttributes - 1; a++)
											trainingStreamWriter.Write($"{dataSet[v][a]:F5} ");
									}
								}
								else
								{
									for (int a = 0; a < _numAttributes - 1; a++)
										trainingStreamWriter.Write($"{dataSet[v][a]:F5} ");
								}

								if (_noiseOutput)
								{
									if (Classification)
									{

										if ((r = R.NextDouble()) < NoiseFrequencyOut)
										{
											trainingStreamWriter.Write($"{dataSet[R.Next(_numVectors)][_numAttributes - 1]:F5} ");
										}
										else
										{
											trainingStreamWriter.Write($"{dataSet[v][_numAttributes - 1]:F5} ");
										}

									}
									else
									{

										if ((r = R.NextDouble()) < NoiseFrequencyOut)
										{
											trainingStreamWriter.Write($"{(dataSet[v][_numAttributes - 1] + NoiseValue * 1.72 * (R.NextDouble() + R.NextDouble() + R.NextDouble() + R.NextDouble() - 2)):F5} ");
										}
										else
										{
											trainingStreamWriter.Write($"{dataSet[v][_numAttributes - 1]:F5} ");
										}
									}
								}
								else
								{
									trainingStreamWriter.Write($"{dataSet[v][_numAttributes - 1]:F5} ");
								}

								trainingStreamWriter.WriteLine();
							}
						}
					}
					else
					{
						for (int v = 0; v < _numVectors; v++)
						{
							if (v >= steps[f] && v < steps[f + 1]) //write these vectors to the test set
							{
								testStreamWriter.WriteLine(originalFile[v + 1]);
							}
							else //write these vectors to the training set
							{
								trainingStreamWriter.WriteLine(originalFile[v + 1]);
							}
						}
					}
				}



			}  // fold

		}
	}
}
