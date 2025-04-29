using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using Unity.VisualScripting;

public class UIManager : MonoSingleton<UIManager>, IUIManager, IInitializable, IInjectable
{
    public int Priority => 0;
    public bool AutoInitialize => true;
    public Type[] GetDependencies() => Array.Empty<Type>();

    private readonly Dictionary<string, UIBase> uiList = new();

    private GameObject globalUICanvasGO;
    private Canvas globalUICanvas;

    public async UniTask InitializeAsync()
    {
        await UniTask.Delay(100);
    }

    // 일반 UI Show
    public async UniTask<T> ShowAsync<T>(params object[] param) where T : UIBase
    {
        string uiName = typeof(T).ToString();
        var result = await ShowInternalAsync(uiName, param);
        return result as T;
    }

    public async UniTask<UIBase> ShowByNameAsync(string uiName)
    {
        return await ShowInternalAsync(uiName, null);
    }

    private async UniTask<UIBase> ShowInternalAsync(string uiName, object[] param)
    {
        if (uiList.TryGetValue(uiName, out UIBase uI))
        {
            if (uI == null || uI.canvas == null || uI.canvas.gameObject == null)
            {
                Debug.LogWarning($"[UIManager] UI reference found but invalid: {uiName}. Recreating...");
                uiList.Remove(uiName); // 캐시 제거
            }
            else
            {
                uI.Opened(param);
                return uI;
            }
        }

        // Addressables에서 프리팹 로드
        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>($"{uiName}");

        if (prefab == null)
        {
            Debug.LogError($"[UIManager] UI Prefab not found in Addressables: {uiName}");
            return null;
        }

        // Canvas 생성
        var canvasGO = new GameObject(uiName + "Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 760);
        canvasGO.AddComponent<GraphicRaycaster>();

        // 프리팹 인스턴스화
        var obj = GameObject.Instantiate(prefab, canvasGO.transform);
        obj.name = uiName;

        uI = obj.GetComponent<UIBase>();
        if (uI == null)
        {
            Debug.LogError($"[UIManager] UI Prefab missing UIBase component: {uiName}");
            return null;
        }

        uI.canvas = canvas;
        uiList[uiName] = uI;

        if (uI.IsGlobalUI())
            DontDestroyOnLoad(canvasGO);

        // 정렬 순서 적용
        uI.canvas.sortingOrder = uiList.Count;

        //DI 주입(하위 컴포넌트까지 포함)
        var registry = SceneServiceManager.CurrentRegistry;
        if (registry != null)
            InjectRunner.TryInjectAll(obj.gameObject, registry.AsDictionary()); // MonoBehaviour 전체


        uI.Opened(param);

        return uI;
    }

    public void Hide<T>() where T : UIBase
    {
        Hide(typeof(T).ToString());
    }

    public void Hide(string uiName)
    {
        if (!uiList.TryGetValue(uiName, out var ui))
            return;

        if (ui.canvas != null)
        {
            if (Application.isPlaying)
                GameObject.Destroy(ui.canvas.gameObject);
            else
                GameObject.DestroyImmediate(ui.canvas.gameObject);
        }

        uiList.Remove(uiName);
    }

    public void ActivateGlobalUI(string uiKey)
    {
        if (uiList.TryGetValue(uiKey, out var ui))
        {
            ui.canvas.gameObject.SetActive(true);
            ui.Opened(null);
        }
    }

    public void DeactivateGlobalUI(string uiKey)
    {
        if (uiList.TryGetValue(uiKey, out var ui))
        {
            ui.canvas.gameObject.SetActive(false);
        }
    }

    // 간단한 텍스트 메시지를 화면 중앙에 표시
    public async void ShowMessage(string message, float duration = 2f)
    {
        // Canvas 찾기 또는 생성
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("PopupCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasGO);
        }

        // 텍스트 오브젝트 생성
        GameObject textGO = new GameObject("PopupMessage");
        textGO.transform.SetParent(canvas.transform, false);

        Text text = textGO.AddComponent<Text>();
        text.text = message;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 32;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(800, 100);

        // 일정 시간 후 제거
        await UniTask.Delay((int)(duration * 1000));
        if (textGO != null)
        {
            Destroy(textGO);
        }
    }
}