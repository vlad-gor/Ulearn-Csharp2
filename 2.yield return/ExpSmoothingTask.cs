// Вставьте сюда финальное содержимое файла ExpSmoothingTask.cs
using System.Collections.Generic;
 
namespace yield
{
    public static class ExpSmoothingTask
    {
        public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
        {
            var isFirstItem = true;
            double previousItem = 0;
            foreach (var e in data)
            {
                var item = new DataPoint();
                item.AvgSmoothedY = e.AvgSmoothedY;
                item.MaxY = e.MaxY;
                item.OriginalY = e.OriginalY;
                item.X = e.X;
                if(isFirstItem)
                {
                    isFirstItem = false;
                    previousItem = e.OriginalY;
                }
                else
                {
                    previousItem = alpha * e.OriginalY + (1 - alpha) * previousItem;
                }
                item.ExpSmoothedY = previousItem;
                yield return item;
            }
        }
    }
}