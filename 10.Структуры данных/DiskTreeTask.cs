using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree
{
    public class DiskTreeTask
    {
        public class Root
        {
            public string Name;
            public Dictionary<string, Root> Nodes = new Dictionary<string, Root>();

            public Root(string name)
            {
                Name = name;
            }

            public Root GetDirection(string subRoot)
            {
                return Nodes.TryGetValue(subRoot, out Root node) 
					? node : Nodes[subRoot] = new Root(subRoot);
            }

            public List<string> MakeConcluson(int i, List<string> list)
            {
                if (i >= 0)
                    list.Add(new string(' ', i) + Name);
                i++;

                foreach (var child in Nodes.Values.OrderBy(root => root.Name, 
														   StringComparer.Ordinal))
                    list = child.MakeConcluson(i, list);
                return list;
            }
        }

        public static List<string> Solve(List<string> input)
        {
            var root = new Root("");
            foreach (var name in input)
            {
                var path = name.Split('\\');
                var node = root;
                foreach (var item in path)
                    node = node.GetDirection(item);
            }

            return root.MakeConcluson(-1, new List<string>());
        }
    }
}