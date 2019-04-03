﻿using System;
using System.Collections.Generic;
using Microsoft.ML.Data;

namespace Microsoft.ML.Samples.Dynamic
{
    public static class SelectFeaturesBasedOnCount
    {
        public static void Example()
        {
            // Create a new ML context, for ML.NET operations. It can be used for exception tracking and logging, 
            // as well as the source of randomness.
            var mlContext = new MLContext();

            // Get a small dataset as an IEnumerable and convert it to an IDataView.
            var rawData = GetData();
            var data = mlContext.Data.LoadFromEnumerable(rawData);

            Console.WriteLine("Contents of column 'GroupB'");
            PrintDataColumn(data, "GroupB");
            // 4       NaN     6
            // 4       5       6
            // 4       5       6
            // 4       NaN     NaN

            // Second, we define the transformations that we apply on the data. Remember that an Estimator does not transform data
            // directly, but it needs to be trained on data using .Fit(), and it will output a Transformer, which can transform data.

            // We will use the SelectFeaturesBasedOnCount transform estimator, to retain only those slots which have 
            // at least 'count' non-default values per slot.
            var pipeline = mlContext.Transforms.FeatureSelection.SelectFeaturesBasedOnCount(
                outputColumnName: "FeaturesSelectedGroupB", inputColumnName: "GroupB", count: 3);

            // The pipeline can then be trained, using .Fit(), and the resulting transformer can be used to transform data. 
            var transformedData = pipeline.Fit(data).Transform(data);

            Console.WriteLine("Contents of column 'FeaturesSelectedGroupB'");
            PrintDataColumn(transformedData, "FeaturesSelectedGroupB");
            // 4       6
            // 4       6
            // 4       6
            // 4       NaN
        }

        private static void PrintDataColumn(IDataView transformedData, string columnName)
        {
            var countSelectColumn = transformedData.GetColumn<float[]>(transformedData.Schema[columnName]);

            foreach (var row in countSelectColumn)
            {
                for (var i = 0; i < row.Length; i++)
                    Console.Write($"{row[i]}\t");
                Console.WriteLine();
            }
        }

        public class NumericData
        {
            public bool Label;

            [VectorType(3)]
            public float[] GroupA { get; set; }

            [VectorType(3)]
            public float[] GroupB { get; set; }

            [VectorType(3)]
            public float[] GroupC { get; set; }
        }

        /// <summary>
        /// Returns a few rows of numeric data.
        /// </summary>
        public static IEnumerable<NumericData> GetData()
        {
            var data = new List<NumericData>
            {
                new NumericData
                {
                    Label = true,
                    GroupA = new float[] { 1, 2, 3 },
                    GroupB = new float[] { 4, float.NaN, 6 },
                    GroupC = new float[] { 7, 8, 9 },
                },
                new NumericData
                {
                    Label = false,
                    GroupA = new float[] { 1, 2, 3 },
                    GroupB = new float[] { 4, 5, 6 },
                    GroupC = new float[] { 7, 8, 9 },
                },
                new NumericData
                {
                    Label = true,
                    GroupA = new float[] { 1, 2, 3 },
                    GroupB = new float[] { 4, 5, 6 },
                    GroupC = new float[] { 7, 8, 9 },
                },
                new NumericData
                {
                    Label = false,
                    GroupA = new float[] { 1, 2, 3 },
                    GroupB = new float[] { 4, float.NaN, float.NaN },
                    GroupC = new float[] { 7, 8, 9 },
                }
            };
            return data;
        }
    }
}
