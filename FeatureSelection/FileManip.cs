using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FeatureSelection
{
	class FileManip
	{
		public static double[][] LoadDataset(string path)
		{
			int columns = 0;
			int lines = 1;

			if (File.Exists(path))
			{
				double[][] returnArray;

				using (StreamReader reader = new StreamReader(path))
				{
					// Change the separator to '.'
					string localizationOldSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

					System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
					customCulture.NumberFormat.NumberDecimalSeparator = ".";

					System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
					// End of culture

					// Using reader.ReadLine() here ignores the first line in file
					columns = reader.ReadLine().Split(' ').Length - 2;

					for (string line = reader.ReadLine(); line != null && line != ""; line = reader.ReadLine())
						lines++;

					returnArray = new double[lines - 1][];
					
					// Returns to the beginning of the file
					reader.DiscardBufferedData();
					reader.BaseStream.Seek(0, SeekOrigin.Begin);
					reader.BaseStream.Position = 0;

					// Ignores the first line
					reader.ReadLine();

					// Initializes the elements of returnArray[]
					for (int i = 1; i < lines; i++)
						returnArray[i - 1] = new double[columns];

					// Loads the things into the final thing
					for (int i = 0; i < lines - 1; i++)
					{
						string[] thisLine = reader.ReadLine().Split(' ');

						for (int j = 0; j < columns; j++)
						{
							returnArray[i][j] = Convert.ToDouble(thisLine[j]);
						}
					}

					// Restore the old separator
					customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
					customCulture.NumberFormat.NumberDecimalSeparator = localizationOldSeparator;

					System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
					// End of culture

					return returnArray;
				}
			}
			else
				throw new FileNotFoundException(path);
		}
	}
}
