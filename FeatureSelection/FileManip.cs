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
		static double[][] LoadDataset(string path, out int lines, out int columns)
		{
			columns = 0;
			lines = 1;

			if (File.Exists(path))
			{
				double[][] returnArray;

				using (StreamReader reader = new StreamReader(path))
				{
					// Using reader.ReadLine() here ignores the first line in file
					columns = reader.ReadLine().Split(' ').Length - 2;

					returnArray = new double[columns][];

					while (reader.ReadLine() != null)
						lines++;

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
					for (int i = 0; reader.Peek() != -1; i++)
					{
						string[] thisLine = reader.ReadLine().Split(' ');

						for (int j = 0; j < columns; j++)
						{
							returnArray[i][j] = Convert.ToDouble(thisLine[j]);
						}
					}

					return returnArray;
				}
			}
			else
			{
				columns = 0;
				return null;
			}
		}
	}
}
