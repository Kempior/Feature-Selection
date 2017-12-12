using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection
{
	abstract class Filter
	{
		public abstract double[][] FilterData(double[][] data, uint remaining);
		public double[][] FilterData(double[][] data)
		{
			return FilterData(data, 0);
		}
	}
}
