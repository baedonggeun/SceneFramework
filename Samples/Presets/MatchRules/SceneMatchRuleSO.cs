using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "HybridSceneFramework/Scene Match Rule")]
public class SceneMatchRuleSO : ScriptableObject
{
    [System.Serializable]
    public class SceneMatchEntry
    {
        public string sceneName;       // 예: "IntroScene", "GameScene"
        public SceneType sceneType;    // 예: Intro, Game, Result
    }

    [Header("씬 이름 ↔ SceneType 매핑")]
    [SerializeField] private List<SceneMatchEntry> entries = new();

    private Dictionary<string, SceneType> lookup;

    public SceneType GetSceneType(string sceneName)
    {
        if (lookup == null)
        {
            lookup = entries.ToDictionary(e => e.sceneName, e => e.sceneType);
        }

        return lookup.TryGetValue(sceneName, out var result)
            ? result
            : SceneType.Intro;  // 기본값 또는 throw
    }

#if UNITY_EDITOR
    public void RebuildLookup()
    {
        lookup = entries.ToDictionary(e => e.sceneName, e => e.sceneType);
    }
#endif
}