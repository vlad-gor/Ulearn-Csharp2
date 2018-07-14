using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            //Путь до выхода
            var pathToExit = BfsTask.FindPaths(map, map.InitialPosition, new[] { map.Exit }).FirstOrDefault();
            //Если нет пути до выхода
            if (pathToExit == null)
                return new MoveDirection[0];

            //Если найденый путь до выхода содержит хоть один сундук
            if (map.Chests.Any(chest => pathToExit.ToList().Contains(chest)))
                return pathToExit.ToList().ParseToDirections();

            //Находим кратчайший путь
            var chests = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
            var result = chests.Select(chest => Tuple.Create(
                chest, BfsTask.FindPaths(map, chest.Value, new[] { map.Exit }).FirstOrDefault()))
                .MinElement();
            //Если кратчайший путь не проходит ни через один сундук
            if (result == null) return pathToExit.ToList().ParseToDirections();

            //Парсим каждую часть пути (до сундука и от него) в путь MoveDirection и соединяем
            return result.Item1.ToList().ParseToDirections().Concat(
                result.Item2.ToList().ParseToDirections())
                .ToArray();
        }
    }

    public static class ExtentionMetods
    {
        //Поиск минимального пути, к котором путь до сундука и от него до выхода
        //Суммарно будут кратчайшими
        public static Tuple<SinglyLinkedList<Point>, SinglyLinkedList<Point>>
            MinElement(this IEnumerable<Tuple<SinglyLinkedList<Point>, SinglyLinkedList<Point>>> items)
        {
            if (items.Count() == 0 || items.First().Item2 == null)
                return null;

            var min = int.MaxValue;
            var minElement = items.First();
            foreach (var element in items)
                if (element.Item1.Length + element.Item2.Length < min)
                {
                    min = element.Item1.Length + element.Item2.Length;
                    minElement = element;
                }
            return minElement;
        }

        //Перевод из последовательности точек к последовательность направлений
        public static MoveDirection[] ParseToDirections(this List<Point> items)
        {
            var resultList = new List<MoveDirection>();
            if (items == null)
                return new MoveDirection[0];
            var itemsLength = items.Count;

            for (var i = itemsLength - 1; i > 0; i--)
            {
                resultList.Add(GetDirection(items[i], items[i - 1]));
            }
            return resultList.ToArray();
        }

        //Направление между двумя точками
        static MoveDirection GetDirection(Point firstPoint, Point secondPoint)
        {
            var newPoint = new Point(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y);
            if (newPoint.X == 1) return MoveDirection.Left;
            if (newPoint.X == -1) return MoveDirection.Right;
            if (newPoint.Y == 1) return MoveDirection.Up;
            if (newPoint.Y == -1) return MoveDirection.Down;
            throw new ArgumentException();
        }
    }
}