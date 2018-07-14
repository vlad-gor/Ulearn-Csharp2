using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
    public class NotGreedyPathFinder : IPathFinder
    {
        public const int Limiter = 8;

        public List<Point> FindPathToCompleteGoal(State state)
        {
            var graph = new Graph(state.MapHeight * state.MapWidth);
            var weights = new Dictionary<Edge, double>();
            var chests = state.Chests.Take(Limiter);

            var pathsBetweenChests = new Dictionary<Point, Dictionary<Point, List<Point>>>();

            InitializeGraphOnCells(state, weights, graph);

            pathsBetweenChests[state.Position] = GetPathToAllChests(graph, weights, state.Position, state);

            foreach (var chest in chests)
                pathsBetweenChests[chest] = GetPathToAllChests(graph, weights, chest, state);

            var bestPath = new List<Point>();

            for (var i = 1; i <= Limiter; i++)
            {
                var reachableChestPermutations = GetPermutations(chests, i)
                    .Where(perm => PathsPermutationStaminaCost(state, perm, pathsBetweenChests) <= state.Energy);

                if (!reachableChestPermutations.Any())
                    break;

                bestPath = reachableChestPermutations
                    .First()
                    .ToList();
            }


            return GetTotalPathByPermutation(state, bestPath, pathsBetweenChests);
        }

        private static Dictionary<Point, List<Point>> GetPathToAllChests(Graph graph, Dictionary<Edge, double> weights,
            Point start, State state)
        {
            var result = new Dictionary<Point, List<Point>>();
            var chests = state.Chests.Where(chest => chest != start);

            foreach (var chest in chests)
                result[chest] = GetPathToChest(graph, weights, state, start, chest);

            return result;
        }

        private static List<Point> GetPathToChest(Graph graph, Dictionary<Edge, double> weights,
            State state, Point start, Point chest)
        {
            var initialPositionNumber = GetPointNumber(start, state.MapWidth);
            var chestPositionNumber = GetPointNumber(chest, state.MapWidth);

            var path = Dijkstra(graph, weights, graph[initialPositionNumber], graph[chestPositionNumber]);

            return path?.Select(n => CreatePointByNumber(n.NodeNumber, state.MapWidth)).Skip(1).ToList() ??
                   new List<Point>();
        }

        private static int PathStaminaCost(State state, IEnumerable<Point> path)
        {
            return path.Sum(point => state.CellCost[point.X, point.Y]);
        }

        private static int PathsPermutationStaminaCost(State state, IEnumerable<Point> chests,
            Dictionary<Point, Dictionary<Point, List<Point>>> pathsBetweenChests)
        {
            var current = state.Position;
            var total = 0;

            foreach (var chest in chests)
            {
                total += PathStaminaCost(state, pathsBetweenChests[current][chest]);
                current = chest;
            }

            return total;
        }

        private static List<Point> GetTotalPathByPermutation(State state, ICollection<Point> pointsPermutation,
            Dictionary<Point, Dictionary<Point, List<Point>>> pathsBetweenChests)
        {
            var current = state.Position;
            var result = new List<Point>();

            foreach (var point in pointsPermutation)
            {
                result.AddRange(pathsBetweenChests[current][point]);
                current = point;
            }

            return result;
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new[] { t2 }));
        }

        private static void InitializeGraphOnCells(State state, IDictionary<Edge, double> weights, Graph graph)
        {
            for (var y = 0; y < state.MapHeight; y++)
                for (var x = 0; x < state.MapWidth; x++)
                {
                    var point = new Point(x, y);

                    for (var dy = -1; dy <= 1; dy++)
                        for (var dx = -1; dx <= 1; dx++)
                        {
                            if (dx != 0 && dy != 0) continue;
                            var neighbour = new Point(x + dx, y + dy);
                            if (!state.InsideMap(neighbour)) continue;
                            if (state.IsWallAt(neighbour)) continue;

                            var pointNumber = GetPointNumber(point, state.MapWidth);
                            var neighbourNumber = GetPointNumber(neighbour, state.MapWidth);
                            weights[graph.Connect(pointNumber, neighbourNumber)] = state.CellCost[neighbour.X, neighbour.Y];
                        }
                }
        }

        private static int GetPointNumber(Point point, int mapWidth)
        {
            return point.Y * mapWidth + point.X;
        }

        private static Point CreatePointByNumber(int pointNumber, int mapWidth)
        {
            return new Point(pointNumber % mapWidth, pointNumber / mapWidth);
        }

        private static List<Node> Dijkstra(Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
        {
            var notVisited = graph.Nodes.ToList();
            var track = new Dictionary<Node, DijkstraData>();
            track[start] = new DijkstraData { Price = 0, Previous = null };

            while (true)
            {
                Node toOpen = null;
                var bestPrice = double.PositiveInfinity;
                foreach (var e in notVisited)
                    if (track.ContainsKey(e) && track[e].Price < bestPrice)
                    {
                        bestPrice = track[e].Price;
                        toOpen = e;
                    }

                if (toOpen == null) return null;
                if (toOpen == end) break;

                foreach (var e in toOpen.IncidentEdges.Where(z => z.From == toOpen))
                {
                    var currentPrice = track[toOpen].Price + weights[e];
                    var nextNode = e.OtherNode(toOpen);
                    if (!track.ContainsKey(nextNode) || track[nextNode].Price > currentPrice)
                        track[nextNode] = new DijkstraData { Previous = toOpen, Price = currentPrice };
                }

                notVisited.Remove(toOpen);
            }

            var result = new List<Node>();
            while (end != null)
            {
                result.Add(end);
                end = track[end].Previous;
            }
            result.Reverse();
            return result;
        }

        internal class DijkstraData
        {
            public Node Previous { get; set; }
            public double Price { get; set; }
        }

        internal class Edge
        {
            public readonly Node From;
            public readonly Node To;

            public Edge(Node first, Node second)
            {
                From = first;
                To = second;
            }

            public bool IsIncident(Node node)
            {
                return From == node || To == node;
            }

            public Node OtherNode(Node node)
            {
                if (!IsIncident(node)) throw new ArgumentException();
                if (From == node) return To;
                return From;
            }
        }

        internal class Node
        {
            private readonly List<Edge> edges = new List<Edge>();
            public readonly int NodeNumber;

            public Node(int number)
            {
                NodeNumber = number;
            }

            public IEnumerable<Node> IncidentNodes
            {
                get { return edges.Select(z => z.OtherNode(this)); }
            }

            public IEnumerable<Edge> IncidentEdges
            {
                get
                {
                    foreach (var e in edges) yield return e;
                }
            }

            public static Edge Connect(Node node1, Node node2, Graph graph)
            {
                if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
                var edge = new Edge(node1, node2);
                node1.edges.Add(edge);
                return edge;
            }
        }

        internal class Graph
        {
            private readonly Node[] nodes;

            public Graph(int nodesCount)
            {
                nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();
            }

            public int Length => nodes.Length;

            public Node this[int index] => nodes[index];

            public IEnumerable<Node> Nodes
            {
                get
                {
                    foreach (var node in nodes) yield return node;
                }
            }

            public IEnumerable<Edge> Edges
            {
                get { return nodes.SelectMany(z => z.IncidentEdges).Distinct(); }
            }

            public Edge Connect(int index1, int index2)
            {
                return Node.Connect(nodes[index1], nodes[index2], this);
            }

            public static Graph MakeGraph(params int[] incidentNodes)
            {
                var graph = new Graph(incidentNodes.Max() + 1);
                for (var i = 0; i < incidentNodes.Length - 1; i += 2)
                    graph.Connect(incidentNodes[i], incidentNodes[i + 1]);
                return graph;
            }
        }
    }
}