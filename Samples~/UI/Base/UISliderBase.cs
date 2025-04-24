using UnityEngine;
using UnityEngine.UI;

// 모든 슬라이더형 기능 UI의 공통 베이스 클래스
// onValueChanged 자동 연결 구조를 제공
[RequireComponent(typeof(Slider))]
public abstract class UISliderBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        var slider = GetComponent<Slider>();
        if (slider != null)
            slider.onValueChanged.AddListener(OnValueChanged);
    }

    // 슬라이더 값이 변경될 때 호출될 기능은 하위 클래스에서 구현
    protected abstract void OnValueChanged(float value);
}