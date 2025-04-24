using TMPro;
using UnityEngine;

// 공통 텍스트 제어용 베이스 클래스
// TextMeshProUGUI를 내부에서 관리하고, SetText 메서드를 통해 갱신
[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class UITextBase : MonoBehaviour
{
    protected TextMeshProUGUI text;

    protected virtual void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    protected void SetText(string value)
    {
        if (text != null)
            text.text = value;
    }
}
