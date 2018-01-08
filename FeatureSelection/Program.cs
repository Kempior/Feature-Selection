using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection
{
	class Program
	{
		static void Main(string[] args)
		{
			const int fold = 10;

			// Hardcoded for now, too lazy to actually do it for now
			string filePath = Environment.CurrentDirectory + "\\..\\..\\TestExamples";

			// Runs the crossvalidation thing
			Crossvalidation.SpiltData(filePath + "\\machineCPU_std_sh.txt", fold);

			// Loads the Trainingdataset and testDataset
			double[][][] trainingDataset = new double[fold][][];
			double[][][] testDataset = new double[fold][][];
			for (int i = 0; i < fold; i++)
			{
				trainingDataset[i] = FileManip.LoadDataset(filePath + $"\\machineCPU_std_sh\\machineCPU_std_sh_10_{i}_trn.txt", 2);
				testDataset[i] = FileManip.LoadDataset(filePath + $"\\machineCPU_std_sh\\machineCPU_std_sh_10_{i}_tst.txt", 2);
			}

			// Creates a fiter that will be used further
			Filter filter = new CorrelationFilter();

			// Uses the filter to... filter the data i guess
			for (int i = 0; i < fold; i++)
			{
				trainingDataset[0] = filter.FilterTrainingData(trainingDataset[0], 2);
				testDataset[0] = filter.FilterTestData(testDataset[0]);
			}


			// Does its neural magicks... seriously, i have no idea what it does, i've just copied it from prof. Kordos
			ISNN.LearningAlgorithms.Knn[] knn = new ISNN.LearningAlgorithms.Knn[fold];
			for (int i = 0; i < fold; i++)
				knn[i] = new ISNN.LearningAlgorithms.Knn();

			for (int i = 0; i < fold; i++)
			{
				knn[i].GetDistances(trainingDataset[i], testDataset[i]);
				double rmsse_Acc = knn[i].PredictEvol(0);
				Console.WriteLine(rmsse_Acc);
			}

			Console.ReadKey();
		}
	}
}
