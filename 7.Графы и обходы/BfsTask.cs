// Вставьте сюда финальное содержимое файла BfsTask.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Dungeon
{
    public class BfsTask
    {
        public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
        {
            var queue = new Queue<Point>();
            var visited = new HashSet<Point>();
            var ways = new Dictionary<Point, SinglyLinkedList<Point>>();
 
            visited.Add(start);
            queue.Enqueue(start);
            ways.Add(start, new SinglyLinkedList<Point>(start));
            
            while (queue.Count != 0)
            {
                var point = queue.Dequeue();
                if (point.X < 0 || point.X >= map.Dungeon.GetLength(0) || point.Y < 0 || point.Y >= map.Dungeon.GetLength(1)) continue;
                if (map.Dungeon[point.X, point.Y] != MapCell.Empty) continue;
 
                for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    if (dx != 0 && dy != 0) continue;
                    var nextPoint = new Point {X = point.X + dx, Y = point.Y + dy}; 
 
                    if (visited.Contains(nextPoint)) continue;
 
                    queue.Enqueue(nextPoint);
                    visited.Add(nextPoint);
                    ways.Add(nextPoint, new SinglyLinkedList<Point>(nextPoint, ways[point]));
                }
            }
 
            foreach (var chest in chests)
            {
                if (ways.ContainsKey(chest)) yield return ways[chest];
            }
        }
    }
}