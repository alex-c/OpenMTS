using OpenMTS.Models.Environmnt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Services.Environment
{
    /// <summary>
    /// A data density reducer implementation based on the Ramer-Douglas-Peucker algorithm.
    /// </summary>
    public class RamerDouglasPeuckerDataDensityReducer : IDataDensityReducer
    {
        /// <summary>
        /// Reduces the data density in order to return less than <paramref name="maxPoint"/> data points.
        /// </summary>
        /// <param name="data">The source dataset to reduce.</param>
        /// <param name="maxPoints">The upper bound of points for the result dataset.</param>
        /// <returns>Returns a reduced dataset.</returns>
        public IEnumerable<DataPoint> ReduceDensity(IEnumerable<DataPoint> data, int maxPoints)
        {
            // Transform data set for easier handling
            Point[] transformedData = data.Select(d => new Point(d)).ToArray();

            // Choose an epsilon and perform initial reduction
            double epsilon = data.Count() / maxPoints * 0.1;
            IEnumerable<Point> reducedPoints = RecursiveRamerDouglasPeucker(transformedData, 0, data.Count() - 1, epsilon);

            // Iteratively approach reduction goal with upper bound of 10 iterations
            int i = 0;
            while (reducedPoints.Count() > maxPoints && i++ < 10)
            {
                epsilon *= 1.5;
                reducedPoints = RecursiveRamerDouglasPeucker(transformedData, 0, data.Count() - 1, epsilon);
            }
            return reducedPoints.Select(p => p.Source);
        }

        /// <summary>
        /// Recursive implementation of the RDP algorithm for a given data range and tolerance.
        /// </summary>
        /// <param name="data">The array of points to opperate on.</param>
        /// <param name="startIndex">The start index of the data portion to opperate on.</param>
        /// <param name="endIndex">The end index of the data portion to opperate on.</param>
        /// <param name="epsilon">Distance tolerance - the higher, the more data is removed.</param>
        /// <returns>Returns a reduced dataset.</returns>
        private IEnumerable<Point> RecursiveRamerDouglasPeucker(Point[] data, int startIndex, int endIndex, double epsilon)
        {
            if (startIndex >= endIndex - 1)
            {
                return new Point[] { data[startIndex] };
            }

            // Slope between start and end point
            float slope = (data[endIndex].Y - data[startIndex].Y) / (data[endIndex].X - data[startIndex].X);

            // Start point coords
            long x0 = data[startIndex].X;
            float y0 = data[startIndex].Y;

            // Compute vertical distance for each point and get max
            int indexOfMaxVerticalDistance = startIndex;
            float maxVerticalDistance = -1;
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                Point point = data[i];
                float y = y0 + slope * (point.X - x0);
                float deltaY = Math.Abs(point.Y - y);

                if (deltaY > maxVerticalDistance)
                {
                    maxVerticalDistance = deltaY;
                    indexOfMaxVerticalDistance = i;
                }
            }

            // Check against epsilon
            if (maxVerticalDistance < epsilon)
            {
                return new Point[] { data[startIndex] };
            }

            // Proceed recursively with bot new halves of the graph
            IEnumerable<Point> points_left = RecursiveRamerDouglasPeucker(data, startIndex, indexOfMaxVerticalDistance, epsilon);
            IEnumerable<Point> points_right = RecursiveRamerDouglasPeucker(data, indexOfMaxVerticalDistance, endIndex, epsilon);

            // Concat and return sub-results
            return points_left.Concat(points_right);
        }
    }

    /// <summary>
    /// Helper class used to represent an environmental data point as a point with X and Y components,
    /// where Y is the environmental factor value (eg. temperature) and X the timestamp in the form of ticks.
    /// </summary>
    internal class Point
    {
        /// <summary>
        /// Time component of the original data point in form of ticks.
        /// </summary>
        public long X { get; }

        /// <summary>
        /// Enviromental factor value.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Original data point.
        /// </summary>
        public DataPoint Source { get; }

        /// <summary>
        /// Creates a new instance from a data point object.
        /// </summary>
        /// <param name="source">The source object.</param>
        public Point(DataPoint source)
        {
            X = source.Timestamp.Ticks;
            Y = source.Value.Value;
            Source = source;
        }
    }
}
