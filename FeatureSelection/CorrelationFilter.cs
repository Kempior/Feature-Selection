using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection
{
	class CorrelationFilter : Filter
	{
		// Returns an array of (data.GetLength(0)) by (remaining + 1) of double
		public override double[][] FilterData(double[][] data, uint remaining)
		{
			uint _vectorNumber = (uint)data.GetLength(0);
			uint _vectorSize = (uint)data.GetLength(1);

			// Sets the remaining to the (length of the vector - 1)
			if (remaining > _vectorSize - 1)
				remaining = _vectorSize - 1;
			else if (remaining == 0)
				throw new Exception("No. The Great Correlation Goat does not approve of you not passing number of remaining columns (or passing a 0).");
			
			// Creates an array of coefficients between (n-th column) and (the last column)
			double[] coefficientsArray = Coefficient(data);

			// Copies the array to a tuple (absolute value of coefficient[i], index of that particulr column)
			Tuple<double, int>[] coefficients = new Tuple<double, int>[_vectorSize - 1];
			for (int i = 0; i < _vectorSize - 1; i++)
				coefficients[i] = new Tuple<double, int>(Math.Abs(coefficientsArray[i]), i);
			
			// Orders them by the coefficient
			coefficients = coefficients.OrderBy(i => i.Item1).ToArray();

			// Does not respect the order of initial data
			double[][] returnValue = new double[_vectorNumber][];
			for (int i = 0; i < _vectorNumber; i++)
			{
				returnValue[i] = new double[remaining];

				for (int j = 0; i < remaining; i++)
				{
					returnValue[j][i] = data[coefficients[j].Item2][i];
				}
			}

			return returnValue;
		}

		double[] Coefficient(double[][] data)
		{
			int vectorNumber = data.GetLength(0);
			int vectorSize = data.GetLength(1);

			double[] means = new double[vectorNumber];
			double[] stdDevs = new double[vectorNumber];

			for(int i = 0; i < vectorSize; i++)
			{
				for(int j = 0; j < vectorNumber; j++)
				{
					means[i] += data[j][i];
				}

				means[i] /= vectorNumber;
			}

			for(int i = 0; i < vectorSize; i++)
			{
				for (int j = 0; j < vectorNumber; j++)
				{
					double temp = data[j][i] - means[i];
					stdDevs[i] += temp * temp;
				}

				stdDevs[i] = Math.Sqrt(stdDevs[i] / (vectorNumber - 1));
			}

			double[] coefficients = new double[vectorSize - 1];
			for (int i = 0; i < vectorSize - 1; i++)
			{
				for (int j = 0; j < vectorNumber; j++)
				{
					coefficients[i] += ((data[j][i] - means[i]) / stdDevs[i]) * ((data[j][vectorSize] - means[vectorSize]) / stdDevs[vectorSize]);
				}

				coefficients[i] /= vectorNumber - 1;
			}

			return coefficients;
		}
	}
}
