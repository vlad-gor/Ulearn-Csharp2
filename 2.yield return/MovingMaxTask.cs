using System;
using System.Linq;
using System.Collections.Generic;
 
namespace yield
{
    public static class MovingMaxTask
    {
        public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
        {
            var points = new LinkedList<double>();
            var pointsX = new Queue<double>();
            foreach (var point in data)
            {
                pointsX.Enqueue(point.X);
                if (pointsX.Count > windowWidth && points.First.Value <= pointsX.Dequeue())
                {
                    points.RemoveFirst();
                    points.RemoveFirst();
                }
                while (points.Count != 0 && points.Last.Value < point.OriginalY)
                {
                    points.RemoveLast();
                    points.RemoveLast();
                }
                points.AddLast(point.X);
                points.AddLast(point.OriginalY);
                point.MaxY = points.First.Next.Value;
                yield return point;
            }
        }
    }
}