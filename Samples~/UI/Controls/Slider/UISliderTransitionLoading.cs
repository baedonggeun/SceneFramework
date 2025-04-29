using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderTransitionLoading : UISliderBase
{
    private Slider slider;

    protected override void Awake()
    {
        base.Awake();
        slider = GetComponent<Slider>();
        slider.interactable = false;  // 드래그 비활성화
    }

    protected override void OnValueChanged(float value)
    {
        // 외부 입력을 받지 않으므로 무반응
    }

    public void SetProgress(float progress)
    {
        if (slider == null) return;
        float clamped = Mathf.Clamp01(progress);
        slider.value = clamped;
    }
}