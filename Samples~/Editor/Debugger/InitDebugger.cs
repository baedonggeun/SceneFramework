using System.Collections.Generic;
using UnityEngine;

public static class InitDebugger
{
    public class InitResult
    {
        public string Name;
        public bool Success;
        public float DurationMs;
        public string Group;
    }

    private static readonly List<InitResult> _results = new();

    public static void Log(string name, string group, bool success, float durationMs)
    {
        _results.Add(new InitResult
        {
            Name = name,
            Group = group,
            Success = success,
            DurationMs = durationMs
        });

        string status = success ? "Success" : "Fail";
        Debug.Log($"[Init] [{group}] {name} - {status} ({durationMs:0.0} ms)");
    }

    public static IReadOnlyList<InitResult> GetResults() => _results.AsReadOnly();

    public static void Clear() => _results.Clear();
}