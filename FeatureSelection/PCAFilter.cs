using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace FeatureSelection
{
    internal class PCAFilter : Filter
    {
        private Matrix<double> W;
        
        public override double[][] FilterTrainingData(double[][] data, uint remaining)
        {
            //Calculate mean value for each column
            double[] mean = new double[data[0].Length];

            foreach (double[] row in data)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    mean[i] += row[i];
                }
            }

            for (int i = 0; i < mean.Length; i++)
            {
                mean[i] /= data.Length;
            }
            
            //Create matrix and center data
            double[,] tmpData = new double[data.Length, data[0].Length];
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[0].Length; j++)
                {
                    tmpData[i, j] = data[i][j] - mean[j];
                }
            }
            Matrix<double> B = Matrix<double>.Build.DenseOfArray(tmpData);
            
            //Calculate covariance matrix
            Matrix<double> C = B.TransposeAndMultiply(B).Multiply(1.0 / (data.Length - 1));

            //Eigen value decomposition
            Evd<double> decomp = C.Evd();

            //Sort eigen values and vectors pairs by eigen value
            Pair[] pairs = EigenPairs(decomp.EigenValues.Real().ToArray(), decomp.EigenVectors.ToColumnArrays());
            
            Array.Sort(pairs);
            
            //Calculate eigenvectors energy
            double[] g = new double[pairs.Length];

            for (int i = 0; i < g.Length; i++)
            {
                for (int j = i; j < g.Length; j++)
                {
                    g[j] += pairs[i].Value;
                }
            }
            
            //Select subset
            const double p = 0.9;

            int L = 0;
            for (int i = 0; i < g.Length; i++)
            {
                if (g[i] / g[g.Length - 1] > p)
                {
                    L = i;
                    break;
                }
            }
            
            //Create reduced matrix
            double[][] tmp = new double[L + 1][];
            for (int i = 0; i < pairs.Length; i++)
            {
                tmp[i] = pairs[i].Vector;
            }

            W = Matrix<double>.Build.DenseOfColumnArrays(tmp);

            //Return projected data
            return B.Multiply(W).ToRowArrays();
        }

        public override double[][] FilterTestData(double[][] data)
        {
            //Calculate mean value for each column
            double[] mean = new double[data[0].Length];

            foreach (double[] row in data)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    mean[i] += row[i];
                }
            }

            for (int i = 0; i < mean.Length; i++)
            {
                mean[i] /= data.Length;
            }
            
            //Create matrix and center data
            double[,] tmpData = new double[data.Length, data[0].Length];
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[0].Length; j++)
                {
                    tmpData[i, j] = data[i][j] - mean[j];
                }
            }
            Matrix<double> B = Matrix<double>.Build.DenseOfArray(tmpData);
            
            //Return projected data
            return B.Multiply(W).ToRowArrays();
        }

        private Pair[] EigenPairs(double[] eigenValues, double[][] eigenVectors)
        {
            Pair[] pairs = new Pair[eigenValues.Length];

            for (int i = 0; i < pairs.Length; i++)
            {
                pairs[i].Value = eigenValues[i];
                pairs[i].Vector = eigenVectors[i];
            }
            
            return pairs;
        }

        private struct Pair : IComparable<Pair>
        {
            public double Value;
            public double[] Vector;

            public int CompareTo(Pair other)
            {
                return -Value.CompareTo(other.Value);
            }
        }
    }
}