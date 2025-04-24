using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class DependencyGraphBuilder
{
    public static List<DependencyNodeData> Build(GraphContext context)
    {
        // 1. 타입 수집 및 Inject 필드 분석
        var allTypes = GetInjectableTypes().ToList();
        var injectMap = new Dictionary<Type, List<Type>>();
        var allInjectTargets = new HashSet<Type>();

        foreach (var type in allTypes)
        {
            // [Inject] 필드 기반 주입 대상 수집
            var injectTargets = GetInjectedFieldTypes(type);

            // GetDependencies() 검사
            Type[] declaredDeps = null;

            bool isUnityType = typeof(UnityEngine.Object).IsAssignableFrom(type);
            bool hasDefaultCtor = type.GetConstructor(Type.EmptyTypes) != null;

            if (!isUnityType && hasDefaultCtor && typeof(IInjectable).IsAssignableFrom(type))
            {
                try
                {
                    var instance = Activator.CreateInstance(type) as IInjectable;
                    declaredDeps = instance?.GetDependencies();
                }
                catch
                {
                    declaredDeps = null;
                }
            }

            // 둘 다 없으면 건너뜀
            if (injectTargets.Count == 0 && (declaredDeps == null || declaredDeps.Length == 0))
                continue;

            // 통과한 경우만 포함
            injectMap[type] = injectTargets;
            foreach (var t in injectTargets)
                allInjectTargets.Add(t);
        }

        // 2. 루트 타입 판별 (자신을 Inject하는 타입이 없는 것)
        var rootTypes = injectMap.Keys
            .Where(t => !allInjectTargets.Contains(t))
            .OrderBy(t => t.Name)
            .ToList();

        // 3. 트리 노드 순서대로 생성 (중복 허용)
        var result = new List<DependencyNodeData>();

        float xSpacing = context.XSpacing;
        float ySpacing = context.YSpacing;
        var currentYByDepth = new Dictionary<int, float>();

        var nodeCache = new Dictionary<Type, DependencyNodeData>();

        float reservedY = 0f;

        foreach (var root in rootTypes)
        {
            var rootNode = TraverseInjectTree(
                type: root,
                depth: 0,
                currentYByDepth: currentYByDepth,
                xSpacing: xSpacing,
                ySpacing: ySpacing,
                injectMap: injectMap,
                result: result,
                callStack: new Stack<Type>(),
                parentNode: null
            );

            if (rootNode != null)
            {
                float bottom = Mathf.Max(reservedY, rootNode.Position.y + rootNode.Size.y + ySpacing);
                currentYByDepth[0] = bottom;
                reservedY = bottom;
            }
        }

        return result;
    }

    private static DependencyNodeData TraverseInjectTree(
    Type type,
    int depth,
    Dictionary<int, float> currentYByDepth,
    float xSpacing,
    float ySpacing,
    Dictionary<Type, List<Type>> injectMap,
    List<DependencyNodeData> result,
    Stack<Type> callStack,
    DependencyNodeData parentNode)
    {
        if (callStack.Contains(type))
        {
            Debug.LogWarning($"[DependencyGraphBuilder] 순환 감지됨: {string.Join(" → ", callStack.Select(t => t.Name))} → {type.Name}");
            return null;
        }

        var injectTargets = injectMap.TryGetValue(type, out var targets) ? targets : new List<Type>();
        callStack.Push(type);

        int sectionCount = 1 + injectTargets.Count;
        float sectionHeight = 30f;
        float margin = 10f;
        float dynamicHeight = sectionCount * sectionHeight + margin;
        var parentSize = new Vector2(300f, dynamicHeight);

        if (!currentYByDepth.ContainsKey(0))
            currentYByDepth[0] = 0f;

        float xPos = 0f;
        float yPos = currentYByDepth[0];

        var parentNodeData = new DependencyNodeData(type, injectTargets)
        {
            ParentNode = parentNode,
            Size = parentSize,
            Position = new Vector2(xPos, yPos)
        };

        result.Add(parentNodeData);

        // 자식들 오른쪽에 순차적으로 정렬
        float childX = xPos + parentSize.x + xSpacing;
        float childY = yPos;

        foreach (var targetType in injectTargets)
        {
            int childSectionCount = 2; // 대략적인 기본값 (타입명 + 하나 정도)
            float childHeight = childSectionCount * sectionHeight + margin;

            var childNode = new DependencyNodeData(targetType, new List<Type>())
            {
                ParentNode = parentNodeData,
                Size = new Vector2(300f, childHeight),
                Position = new Vector2(childX, childY)
            };

            result.Add(childNode);
            childY += childHeight + ySpacing;
        }

        // 전체 블럭의 높이를 currentY에 반영해서 다음 부모는 밑으로 가게
        float fullBlockBottom = Mathf.Max(childY, yPos + parentSize.y) + ySpacing;
        currentYByDepth[0] = fullBlockBottom;

        callStack.Pop();
        return parentNodeData;
    }

    private static IEnumerable<Type> GetInjectableTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
            })
            .Where(t => typeof(IInjectable).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
    }

    private static List<Type> GetInjectedFieldTypes(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<InjectAttribute>() != null)
            .Select(f => f.FieldType)
            .ToList();
    }
}