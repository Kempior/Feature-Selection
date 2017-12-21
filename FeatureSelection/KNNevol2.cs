using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNN.LearningAlgorithms
{
	partial class Knn
	{

		private double[][] _trainingDataSet, _testDataSet;
		private int _numVect, _numAttr;
		private bool _classification;
		public double[][] DistancesX;
		private double[][][] _distancesTestTraining;
		public static double[][][] StaticDistancesTestTraining = new double[10][][];
		public static double[][][] StaticOrderTestTraining = new double[10][][];
		public static int StaticTestL;
		public static double[][] StaticTestClassOrY = new double[10][];
		public static double[][] StaticTrainingClassOrY = new double[10][];
		private double[] correlation;

		// Knn  knn = new Knn();
		// knn.GetDistances(trainingDataSet, testDataSet, 0);
		// rmsse_Acc = knn.PredictEvol(0);



		// 1. Calculate and sort distances to training set vectors for each test set vector  
		public void GetDistances(double[][] trainingDataSet, double[][] testDataSet, int N = 0)
		{

			_trainingDataSet = trainingDataSet;

			if (testDataSet == null)
				_testDataSet = trainingDataSet;
			else
				_testDataSet = testDataSet;

			StaticTestL = _testDataSet.Length;
			_numAttr = _testDataSet[0].Length - 3;

			StaticTestClassOrY[N] = new double[StaticTestL];
			StaticTrainingClassOrY[N] = new double[_trainingDataSet.Length];
			correlation = new double[trainingDataSet[0].Length - 3];

			//  for (int i = 0; i < correlation.Length; i++)
			//     correlation[i] = 1.0;



			CalculateDistances(N, testDataSet == null);
		}

		private void CalculateDistances(int N, bool testDataSetEqualsTrainingDataSet)
		{
			int trainingL = _trainingDataSet.Length;

			int testL = trainingL;
			if (!testDataSetEqualsTrainingDataSet)
				testL = Math.Min(StaticTestL, _testDataSet.Length);

			for (int vTraining = 0; vTraining < trainingL; vTraining++)
				StaticTrainingClassOrY[N][vTraining] = _trainingDataSet[vTraining][_numAttr];

			StaticDistancesTestTraining[N] = new double[testL][];
			StaticOrderTestTraining[N] = new double[testL][];



			if (testDataSetEqualsTrainingDataSet)
			{
				for (int i = 0; i < testL; i++)
					StaticDistancesTestTraining[N][i] = new double[trainingL];


				Parallel.For(0, testL, vTest =>
				{
					StaticTestClassOrY[N][vTest] = _testDataSet[vTest][_numAttr];

					StaticOrderTestTraining[N][vTest] = new double[trainingL];

					for (int vTraining = 0; vTraining < trainingL; vTraining++)
						StaticOrderTestTraining[N][vTest][vTraining] = vTraining;



					double d, dr;

					for (int vTraining = 0; vTraining < vTest; vTraining++) // vTest; vTraining++) blad inicjalizaji tablic - niestety zmiana
					{
						d = 0;
						for (int a = 0; a < _numAttr; a++)
						{
							dr = _trainingDataSet[vTraining][a] - _trainingDataSet[vTest][a];
							// dr *= correlation[a];
							d += dr; // * dr;
						}
						StaticDistancesTestTraining[N][vTest][vTraining] = d;
						StaticDistancesTestTraining[N][vTraining][vTest] = d;
					}

					StaticDistancesTestTraining[N][vTest][vTest] = double.MaxValue; //to prevent predicting a vector by itself                   


				});

				Parallel.For(0, testL, vTest =>
				{
					Array.Sort(StaticDistancesTestTraining[N][vTest], StaticOrderTestTraining[N][vTest]);

				});
			}
			else
			{

				Parallel.For(0, testL, vTest =>
				{
					StaticTestClassOrY[N][vTest] = _testDataSet[vTest][_numAttr];

					StaticDistancesTestTraining[N][vTest] = new double[trainingL];
					StaticOrderTestTraining[N][vTest] = new double[trainingL];

					for (int vTraining = 0; vTraining < trainingL; vTraining++)
						StaticOrderTestTraining[N][vTest][vTraining] = vTraining;

					double d, dr;

					for (int vTraining = 0; vTraining < trainingL; vTraining++)
					{
						d = 0;
						for (int a = 0; a < _numAttr; a++)
						{
							dr = _trainingDataSet[vTraining][a] - _testDataSet[vTest][a];
							//dr *= correlation[a];
							d += dr; // * dr;
						}
						StaticDistancesTestTraining[N][vTest][vTraining] = d;
					}

					Array.Sort(StaticDistancesTestTraining[N][vTest], StaticOrderTestTraining[N][vTest]);
				});

			}
			;

		}



		public double PredictEvol(int N)
		{
			int k = 5;
			

			int numClasses = 20;
			bool R2 = false;

			int trainingL = _trainingDataSet.Length;
			double rmse = 0, accuracy = 0;
			int p0, p1, p2;
			if (numClasses > 1)  //classification
			{
				for (int i = 0; i < StaticTestL; i++)
				{
					double[] predictedClass = new double[numClasses + 2];
					for (int n = 0; n < numClasses + 1; n++)
						predictedClass[n] = 0;

					double k0 = 0;
					for (int j = 0; j < trainingL; j++)
					{
						p0 = (int)StaticOrderTestTraining[N][i][j];

						p1 = (int)StaticOrderTestTraining[N][i][j];
						p2 = (int)StaticTrainingClassOrY[N][p1];
						predictedClass[p2]++;  //what about number and outlier in the last columns


						if ((k0++) >= k)
							break;



					}

					double m = predictedClass.Max();
					int p = Array.IndexOf(predictedClass, m);
					if (p == StaticTestClassOrY[N][i])
						accuracy++;



				}
				accuracy /= StaticTestL;
			}
			else  //regression
			{
				double[] Predicted = new double[StaticTestL];
				for (int i = 0; i < StaticTestL; i++)
				{
					double kt = 0;
					double k0 = 0;
					double predicted = 0;
					for (int j = 0; j < trainingL; j++)
					{
						p0 = (int)StaticOrderTestTraining[N][i][j];


						if (kt >= k) break;
						p1 = (int)StaticOrderTestTraining[N][i][j];
						predicted += StaticTrainingClassOrY[N][p1];


						if ((k0++) >= k)
							break;

					}

					Predicted[i] = predicted / k0;   //what about weighted kNN
					double ddd = Predicted[i] - StaticTestClassOrY[N][i];
					rmse += ddd * ddd;



				}

				if (R2)
				{
					double avgP = Predicted.Average();
					double avgY = StaticTestClassOrY[N].Average();

					double rn = 0, rdx = 0, rdy = 0;
					for (int i = 0; i < Predicted.Length; i++)
					{
						rn += (Predicted[i] - avgP) * (StaticTestClassOrY[N][i] - avgY);
						rdx += (Predicted[i] - avgP) * (Predicted[i] - avgP);
						rdy += (StaticTestClassOrY[N][i] - avgY) * (StaticTestClassOrY[N][i] - avgY);
					}

					double r = rn / (Math.Sqrt(rdx) * Math.Sqrt(rdy));
					double r2 = r * r;
					accuracy = r2;

				}
				else
				{

					if (rmse > 0.0000001)
					{
						rmse = Math.Sqrt(rmse / StaticTestL);
						accuracy = rmse;
					}
					else
					{
						// MessageBox.Show($"rmse = {rmse}");
						//  parameters.Stop = true;

					}

				}


			}


			return accuracy; // (accuracy, rmse);
		}


	}
}

