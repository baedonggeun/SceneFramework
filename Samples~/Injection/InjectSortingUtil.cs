using System;
using System.Collections.Generic;
using System.Linq;

public static class InjectSortingUtil
{
    public static List<T> TopologicalSort<T>(List<T> nodes) where T : IInjectable
    {
        List<T> result = new();
        Dictionary<T, bool> visited = new(); // false = 방문 중, true = 방문 완료

        void Visit(T node)
        {
            if (visited.TryGetValue(node, out bool inProgress))
            {
                if (!inProgress)
                    throw new Exception($"순환 의존성 감지됨: {node.GetType().Name}");
                return;
            }

            visited[node] = false; // 방문 중 표시

            foreach (var depType in node.GetDependencies())
            {
                var dep = nodes.FirstOrDefault(n => depType.IsAssignableFrom(n.GetType()));
                if (dep != null) Visit(dep);
            }

            visited[node] = true; // 방문 완료
            result.Add(node);
        }

        foreach (var node in nodes)
        {
            Visit(node);
        }

        return result;
    }
}