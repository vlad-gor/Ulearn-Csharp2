// Вставьте сюда финальное содержимое файла MovingAverageTask.cs
using System.Collections;
using System.Collections.Generic;

namespace yield
{
    public static class MovingAverageTask
    {
        public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
        {
            var windowValues = new WindowValues<double>(windowWidth);
            var windowsSum = 0.0;

            foreach (var dataPoint in data)
            {
                var tailY = windowValues.Length == windowWidth ? windowValues.Tail.Value : 0;
                windowValues.Append(dataPoint.OriginalY);
                windowsSum -= tailY;
                windowsSum += dataPoint.OriginalY;

                yield return CreateDataPoint(dataPoint, windowsSum / windowValues.Length);
            }
        }

        private static DataPoint CreateDataPoint(DataPoint currentDataPoint, double nextSmoothValue)
        {
            return new DataPoint
            {
                X = currentDataPoint.X,
                AvgSmoothedY = nextSmoothValue,
                OriginalY = currentDataPoint.OriginalY
            };
        }
    }

    internal class WindowValues<T> : IEnumerable<T>
    {
        private readonly int capacity;

        public WindowValues(int capacity)
        {
            this.capacity = capacity;
        }

        public int Length { get; private set; }
        public WindowElement<T> Head { get; private set; }
        public WindowElement<T> Tail { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return new WindowElementEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Append(T value)
        {
            if (Head == null)
            {
                Head = new WindowElement<T> {Value = value, Next = null};
                Tail = Head;
                Length++;
                return;
            }

            Head.Next = new WindowElement<T> {Value = value, Next = null};
            Head = Head.Next;
            if (Length < capacity)
            {
                Length++;
                return;
            }

            Tail = Tail.Next;
        }
    }

    internal class WindowElement<T>
    {
        public WindowElement<T> Next;
        public T Value;
    }

    internal class WindowElementEnumerator<T> : IEnumerator<T>
    {
        private readonly WindowValues<T> window;
        private WindowElement<T> currentElement;

        public WindowElementEnumerator(WindowValues<T> window)
        {
            this.window = window;
            currentElement = null;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            currentElement = currentElement == null ? window.Tail : currentElement.Next;
            return currentElement != null;
        }

        public void Reset()
        {
        }

        public T Current => currentElement.Value;

        object IEnumerator.Current => Current;
    }
}