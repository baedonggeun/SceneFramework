using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUIManager
{
    UniTask<T> ShowAsync<T>(params object[] param) where T : UIBase;
    UniTask<UIBase> ShowByNameAsync(string uiName);
    void Hide<T>() where T : UIBase;
    void Hide(string uiName);
    void ActivateGlobalUI(string uiKey);
    void DeactivateGlobalUI(string uiKey);

    void ShowMessage(string message, float duration = 2f);

}