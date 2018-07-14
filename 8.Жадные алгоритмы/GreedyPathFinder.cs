using System.Collections.Generic;
using System.Drawing;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
    public class GreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            if (state.Chests.Count < state.Goal)
            {
                return new List<Point>();
            }

            var unpickedChests = new HashSet<Point>(state.Chests);
            var pickedChestsNumber = 0;

            var path = new List<Point>();
            var currentEnergy = state.InitialEnergy;
            var currentPosition = state.Position;

            while (pickedChestsNumber < state.Goal)
            {
                int wastedEnergy;

                var pathToNextChest = GetPathToNextChest(unpickedChests, currentPosition, state, out wastedEnergy);

                currentEnergy -= wastedEnergy;
                if (currentEnergy < 0 || pathToNextChest == null)
                {
                    return new List<Point>();
                }

                path.AddRange(pathToNextChest);

                if (pathToNextChest.Count > 0)
                {
                    currentPosition = pathToNextChest[pathToNextChest.Count - 1];
                }

                unpickedChests.Remove(currentPosition);
                pickedChestsNumber++;
            }


            return path;
        }

        private List<Point> GetPathToNextChest(HashSet<Point> chests,
                                                Point startPoint, State state, out int wastedEnergy)
        {
            var passedPoints = new Dictionary<Point, MovingData>();
            var markedPoints = new Dictionary<Point, MovingData>();

            markedPoints.Add(startPoint, new MovingData(startPoint, 0));

            Point lastPoint;
            while (true)
            {
                if (markedPoints.Count == 0)
                {
                    wastedEnergy = 0;
                    return null;
                }

                var openingPoint = GetOpeningPoint(markedPoints);

                passedPoints.Add(openingPoint, markedPoints[openingPoint]);
                markedPoints.Remove(openingPoint);

                if (chests.Contains(openingPoint))
                {
                    lastPoint = openingPoint;
                    break;
                }

                foreach (var nextPoint in Environs(openingPoint))
                {
                    if (passedPoints.ContainsKey(nextPoint) || !state.InsideMap(nextPoint) || state.IsWallAt(nextPoint))
                    {
                        continue;
                    }

                    var energyForNextPoint = state.CellCost[nextPoint.X, nextPoint.Y]
                                                + passedPoints[openingPoint].WastedEnergy;

                    UpdateMarkedPoints(markedPoints, nextPoint, energyForNextPoint, openingPoint);
                }
            }

            wastedEnergy = passedPoints[lastPoint].WastedEnergy;

            return GetResult(passedPoints, startPoint, lastPoint);
        }

        private Point GetOpeningPoint(Dictionary<Point, MovingData> markedPoints)
        {
            var openingPoint = default(Point);
            var energyToPoint = int.MaxValue;

            foreach (var point in markedPoints.Keys)
            {
                if (markedPoints[point].WastedEnergy < energyToPoint)
                {
                    openingPoint = point;
                    energyToPoint = markedPoints[point].WastedEnergy;
                }
            }

            return openingPoint;
        }

        private IEnumerable<Point> Environs(Point current)
        {
            for (int x = current.X - 1; x < current.X + 2; x++)
            {
                for (int y = current.Y - 1; y < current.Y + 2; y++)
                {
                    if (x == current.X || y == current.Y)
                    {
                        yield return new Point(x, y);
                    }
                }
            }
        }

        private void UpdateMarkedPoints(Dictionary<Point, MovingData> markedPoints,
                        Point nextPoint, int energyForNextPoint, Point previousPoint)
        {
            if (markedPoints.TryGetValue(nextPoint, out var alreadyMarkedInfo))
            {
                if (alreadyMarkedInfo.WastedEnergy <= energyForNextPoint)
                {
                    return;
                }
            }

            markedPoints[nextPoint] = new MovingData(previousPoint, energyForNextPoint);
        }

        private List<Point> GetResult(Dictionary<Point, MovingData> passedPoints, Point startPoint, Point lastPoint)
        {
            var result = new List<Point>();

            while (lastPoint != startPoint)
            {
                result.Add(lastPoint);
                lastPoint = passedPoints[lastPoint].Previous;
            }

            result.Reverse();

            return result;
        }

        class MovingData
        {
            public Point Previous { get; set; }
            public int WastedEnergy { get; set; }

            public MovingData(Point previous, int wastedEnergy)
            {
                Previous = previous;
                WastedEnergy = wastedEnergy;
            }
        }
    }
}