using UnityEngine;
using UnityEngine.UI;

// 모든 버튼형 기능 UI의 공통 베이스 클래스
// onClick 자동 연결 구조를 제공
[RequireComponent(typeof(Button))]
public abstract class UIButtonBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    // 클릭 시 동작할 기능은 하위 클래스에서 구현
    protected abstract void OnClick();
}
