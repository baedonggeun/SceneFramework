using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

// UI 버튼에서 사용할 수 있는 씬 전이 유틸리티
// 조건이 충족되면 TryTransition, 실패하면 UI 메시지 출력
public class SceneTransitionUIButton : MonoBehaviour, IInjectable
{
    [Inject] private ISceneTransitionManager _sceneTransitionManager;
    [Inject] private IUIManager _uiManager;
    public Type[] GetDependencies() => new Type[] { typeof(ISceneTransitionManager), typeof(IUIManager) };

    [Header("Preset 키 (ScenePresetSO Addressable 키)")]
    public string presetKey;

    [Header("조건 실패 시 출력할 메시지")]
    public string failureMessage = "전이 조건이 충족되지 않았습니다.";

    [Header("연결할 UI 버튼")]
    public Button targetButton;

    private void Reset()
    {
        targetButton = GetComponent<Button>();
    }

    private void Awake()
    {
        if (targetButton != null)
            targetButton.onClick.AddListener(() => TrySceneTransition().Forget());
    }

    private async UniTaskVoid TrySceneTransition()
    {
        if (string.IsNullOrEmpty(presetKey))
        {
            Debug.LogError("[SceneTransitionUIButton] presetKey가 비어 있습니다.");
            return;
        }

        bool success = await _sceneTransitionManager.TryTransition(presetKey);

        if (!success)
        {
            Debug.LogWarning("[SceneTransitionUIButton] 조건 미충족");
            if (!string.IsNullOrEmpty(failureMessage))
            {
                _uiManager?.ShowMessage(failureMessage); // 메시지 UI 연동 (선택적)
            }
        }
    }
}
