using OpenMTS.Models.Environmnt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Services.Environment
{
    // TODO: comments
    public class RamerDouglasPeuckerDataDensityReducer : IDataDensityReducer
    {
        public IEnumerable<DataPoint> ReduceDensity(IEnumerable<DataPoint> data)
        {
            // Choose an epsilon
            float min = data.Min(d => d.Value).Value;
            float max = data.Max(d => d.Value).Value;
            float epsilon = 10;
            IEnumerable<Point> reducedPoints = RecursiveRamerDouglasPeucker(data.Select(d => new Point(d)).ToArray(), 0, data.Count() - 1, epsilon);
            int test = reducedPoints.Count();
            return reducedPoints.Select(p => p.Source);
        }

        private IEnumerable<Point> RecursiveRamerDouglasPeucker(Point[] data, int startIndex, int endIndex, float epsilon)
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

    internal class Point
    {
        public long X { get; }

        public float Y { get; }

        public DataPoint Source { get; }

        public Point(DataPoint source)
        {
            X = source.Timestamp.Ticks;
            Y = source.Value.Value;
            Source = source;
        }
    }
}
