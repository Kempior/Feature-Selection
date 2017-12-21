using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection
{
	abstract class Filter
	{
		public abstract double[][] FilterTrainingData(double[][] data, uint remaining);
		public double[][] FilterTrainingData(double[][] data)
		{
			return FilterTrainingData(data, 0);
		}

		public abstract double[][] FilterTestData(double[][] data);
	}
}
