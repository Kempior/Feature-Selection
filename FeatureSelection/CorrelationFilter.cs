using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection
{
	class CorrelationFilter : Filter
	{
		private bool[] _filter;

		// Returns an array of (data.GetLength(0)) by (remaining + 1) of double
		public override double[][] FilterTrainingData(double[][] data, uint remaining)
		{
			uint _vectorNumber = (uint)data.Length;
			uint _vectorSize = (uint)data[0].Length;

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

			_filter = new bool[_vectorSize];
			for (int i = 0; i < _filter.Length; i++)
				_filter[i] = false;

			for (int i = 0; i < remaining; i++)
				_filter[coefficients[i].Item2] = true;

			_filter[_vectorSize - 1] = true;

			return FilterTestData(data);
		}

		public override double[][] FilterTestData(double[][] data)
		{
			// Gets the data dimensions
			uint _vectorNumber = (uint)data.Length;
			uint _vectorSize = (uint)data[0].Length;

			// Counts the remaining columns
			// counts the y column
			int remaining = 0;
			foreach (var item in _filter)
				if (item == true)
					remaining++;

			// Initializes the return array
			double[][] returnVal = new double[_vectorNumber][];

			// Initializes return array elements and fills them
			for (int i = 0; i < _vectorNumber; i++)
			{
				returnVal[i] = new double[remaining];
			}

			for (int i = 0, j = 0; i < remaining; i++, j++)
			{
				// Could crash here i suppose, fix it
				while (_filter[j] == false)
					j++;

				for (int k = 0; k < _vectorNumber; k++)
				{
					returnVal[k][i] = data[k][j];
				}
			}

			return returnVal;
		}

		double[] Coefficient(double[][] data)
		{
			int vectorNumber = data.Length;
			int vectorSize = data[0].Length;

			double[] means = new double[vectorSize];
			double[] stdDevs = new double[vectorSize];

			for (int i = 0; i < vectorSize; i++)
			{
				for (int j = 0; j < vectorNumber; j++)
				{
					means[i] += data[j][i];
				}

				means[i] /= vectorNumber;
			}

			for (int i = 0; i < vectorSize; i++)
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
					coefficients[i] += ((data[j][i] - means[i]) / stdDevs[i]) * ((data[j][vectorSize - 1] - means[vectorSize - 1]) / stdDevs[vectorSize - 1]);
				}

				coefficients[i] /= vectorNumber - 1;
			}

			return coefficients;
		}
	}
}
